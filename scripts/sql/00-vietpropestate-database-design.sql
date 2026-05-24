/*
================================================================================
  VietPropEstate — THIẾT KẾ CƠ SỞ DỮ LIỆU SQL SERVER
  Website bất động sản: đăng tin, tìm kiếm, môi giới, VIP, thanh toán, chat
================================================================================

  Tổng quan
  ---------
  - 26 bảng nghiệp vụ + 5 bảng ASP.NET Identity + 1 bảng EF migrations
  - Soft delete: IsDeleted, DeletedAt, DeletedBy (hầu hết entity chính)
  - Audit: CreatedAt/CreatedBy, LastModifiedAt/LastModifiedBy, RowVersion
  - Tiền tệ: PriceAmount + PriceCurrency (Money value object)
  - Địa chỉ VN: Provinces/Wards (mã hành chính) + denormalized trên Properties

  Sơ đồ quan hệ (tóm tắt)
  ------------------------
  AspNetUsers ──┬── Agents ── Properties ──┬── PropertyImages
                │                          ├── Favorites
                ├── Customers              ├── PropertyViews
                ├── UserVIPPackages        ├── Conversations ── Messages
                ├── Payments               └── Transactions
                ├── RefreshTokens
                └── Notifications

  Provinces ── Wards ── (FK) Properties.ProvinceCode / WardCode
  PropertyTypes, TransactionTypes, VIPPackages = lookup tables

  Triển khai
  ----------
  1. scripts/sql/01-create-database.sql          — tạo database
  2. scripts/sql/04-full-schema-idempotent.sql   — tạo toàn bộ schema (EF)
     HOẶC: dotnet ef database update --project src/VietPropEstate.Infrastructure
            --startup-project src/VietPropEstate.WebAPI
  3. Chạy WebAPI với TestSeed:Enabled=true       — seed dữ liệu demo
  4. scripts/sql/03-test-database.sql            — truy vấn kiểm tra

  Enum (int) — tham chiếu từ C# Domain
  ------------------------------------
  PropertyStatus     : Draft=0, Active=1, UnderOffer=2, Sold=3, Rented=4,
                       Withdrawn=5, Expired=6
  ListingType        : ForSale=1, ForRent=2, ForLease=3
  PropertyDirection  : North=1 … SouthWest=8
  CustomerType       : Buyer=1, Seller=2, BuyerAndSeller=3, Tenant=4, Landlord=5
  TransactionStatus  : Pending=0, InProgress=1, Completed=2, Cancelled=3, Refunded=4
  PaymentStatus      : Pending=0, Processing=1, Completed=2, Failed=3,
                       Refunded=4, Cancelled=5
  MessageStatus      : Sent=0, Delivered=1, Read=2, Deleted=3
  NotificationType   : System=0, PropertyAlert=1, MessageReceived=2, …
  AuditAction        : Create=0, Update=1, Delete=2, Login=3, …

================================================================================
  CHI TIẾT BẢNG
================================================================================

-- ── IDENTITY & BẢO MẬT ─────────────────────────────────────────────────────

TABLE AspNetUsers
  Id                  nvarchar(450)   PK — Identity user
  UserName            nvarchar(256)
  NormalizedUserName  nvarchar(256)   UNIQUE (filtered)
  Email               nvarchar(256)
  NormalizedEmail     nvarchar(256)   INDEX
  EmailConfirmed      bit
  PasswordHash        nvarchar(max)
  SecurityStamp       nvarchar(max)
  ConcurrencyStamp    nvarchar(max)
  PhoneNumber         nvarchar(max)
  PhoneNumberConfirmed bit
  TwoFactorEnabled    bit
  LockoutEnd          datetimeoffset
  LockoutEnabled      bit
  AccessFailedCount   int
  -- Mở rộng VietPropEstate:
  FirstName           nvarchar(max)   Tên
  LastName            nvarchar(max)   Họ
  AvatarUrl           nvarchar(max)   Ảnh đại diện
  CreatedAt           datetime2       Ngày tạo tài khoản
  LastLoginAt         datetime2       Lần đăng nhập cuối
  IsActive            bit             Tài khoản còn hoạt động

TABLE AspNetRoles / AspNetUserRoles / AspNetRoleClaims / AspNetUserClaims
TABLE AspNetUserLogins / AspNetUserTokens
  → ASP.NET Core Identity chuẩn (Admin, Broker, Customer, Staff)

TABLE RefreshTokens
  Id                  uniqueidentifier PK
  Token               nvarchar(max)   JWT refresh token
  UserId              nvarchar(450)   FK → AspNetUsers
  ExpiresAt           datetime2
  CreatedAt           datetime2
  CreatedByIp         nvarchar(max)
  IsRevoked           bit
  RevokedAt           datetime2
  RevokedByIp         nvarchar(max)
  RevokedReason       nvarchar(max)
  ReplacedByToken     nvarchar(max)   Token thay thế khi rotate

TABLE AuditLogs
  Id                  uniqueidentifier PK
  UserId              nvarchar(450)   Người thực hiện
  Action              int             AuditAction enum
  EntityName          nvarchar(200)   Tên entity (Property, Payment, …)
  EntityId            nvarchar(450)   Id bản ghi
  OldValues           nvarchar(max)   JSON trước thay đổi
  NewValues           nvarchar(max)   JSON sau thay đổi
  AffectedColumns     nvarchar(max)
  IpAddress           nvarchar(45)
  UserAgent           nvarchar(500)
  Timestamp           datetime2

-- ── ĐỊA CHỈ HÀNH CHÍNH VIỆT NAM ────────────────────────────────────────────

TABLE Provinces
  Code                int             PK — mã tỉnh/thành (VD: 79 = TP.HCM)
  Name                nvarchar(200)   Tên tỉnh/thành
  NameEn              nvarchar(200)   Tên tiếng Anh
  FullName            nvarchar(300)   Tên đầy đủ
  IsActive            bit             DEFAULT 1
  CreatedAt           datetime2
  LastModifiedAt      datetime2

TABLE Wards
  Code                int             PK — mã phường/xã
  Name                nvarchar(200)
  NameEn              nvarchar(200)
  ProvinceId          int             FK → Provinces.Code
  IsActive            bit             DEFAULT 1
  CreatedAt           datetime2
  LastModifiedAt      datetime2
  INDEX (ProvinceId, IsActive)

-- ── LOOKUP / DANH MỤC ──────────────────────────────────────────────────────

TABLE PropertyTypes
  Id                  uniqueidentifier PK
  Name                nvarchar(200)   Căn hộ, Nhà phố, Đất nền, …
  Description         nvarchar(1000)
  IsActive            bit
  + audit + soft delete + RowVersion
  SEED: 6 loại BĐS mặc định

TABLE TransactionTypes
  Id                  uniqueidentifier PK
  Name                nvarchar(200)   Mua bán, Cho thuê, …
  Description         nvarchar(1000)
  IsActive            bit
  + audit + soft delete + RowVersion
  SEED: 3 loại giao dịch

TABLE VIPPackages
  Id                  uniqueidentifier PK
  Name                nvarchar(200)   Gói Basic / Pro / Premium
  Description         nvarchar(2000)
  DurationDays        int             Số ngày hiệu lực
  PriceAmount         decimal(18,2)   Giá gói (VND)
  PriceCurrency       nvarchar(10)    VND
  MaxListings         int             Số tin đăng tối đa
  IsActive            bit
  + audit + soft delete + RowVersion
  SEED: 3 gói VIP

-- ── NGƯỜI DÙNG NGHIỆP VỤ ───────────────────────────────────────────────────

TABLE Agents (Môi giới)
  Id                  uniqueidentifier PK
  FullName            nvarchar(300)
  Email               nvarchar(256)   UNIQUE
  PhoneNumber         nvarchar(20)
  LicenseNumber       nvarchar(100)   Số giấy phép môi giới
  AgencyName          nvarchar(300)   Tên công ty
  AvatarUrl           nvarchar(2000)
  Bio                 nvarchar(3000)
  IsActive            bit
  UserId              nvarchar(450)   FK → AspNetUsers (1 user = 1 agent)
  + audit + soft delete + RowVersion

TABLE Customers (Khách hàng)
  Id                  uniqueidentifier PK
  FullName            nvarchar(300)
  Email               nvarchar(256)
  PhoneNumber         nvarchar(20)
  CustomerType        int             Buyer/Seller/Tenant/…
  Notes               nvarchar(3000)
  UserId              nvarchar(450)   FK → AspNetUsers (optional)
  IsActive            bit
  + audit + soft delete + RowVersion

-- ── BẤT ĐỘNG SẢN (CORE) ────────────────────────────────────────────────────

TABLE Properties
  Id                  uniqueidentifier PK
  Title               nvarchar(500)   Tiêu đề tin
  Slug                nvarchar(600)   URL-friendly, UNIQUE INDEX
  Description         nvarchar(max)   Mô tả chi tiết
  -- Giá:
  PriceAmount         decimal(18,2)
  PriceCurrency       nvarchar(10)    VND, USD, …
  Area                decimal(18,4)   Diện tích m²
  NumberOfBedrooms    int             Phòng ngủ
  NumberOfBathrooms   int             Phòng tắm
  NumberOfFloors      int             Số tầng
  Status              int             PropertyStatus
  ListingType         int             ForSale / ForRent / ForLease
  Direction           int             Hướng nhà (PropertyDirection)
  -- Địa chỉ (value object — cột trên cùng bảng):
  Street              nvarchar(500)
  AddressWard         nvarchar(200)
  AddressDistrict     nvarchar(200)
  AddressProvince     nvarchar(200)
  AddressCountry      nvarchar(100)   DEFAULT Việt Nam
  AddressLatitude     float
  AddressLongitude    float
  -- Địa chỉ VN chuẩn hóa:
  ProvinceCode        int             FK → Provinces.Code
  ProvinceName        nvarchar(200)   Denormalized
  WardCode            int             FK → Wards.Code
  WardName            nvarchar(200)
  FullAddress         nvarchar(1000)  Địa chỉ đầy đủ hiển thị
  Latitude            float           Tọa độ bản đồ
  Longitude           float
  -- Meta:
  ViewCount           int             DEFAULT 0
  IsFeatured          bit             Tin nổi bật
  PublishedAt         datetime2       Ngày đăng
  ExpiresAt           datetime2       Hết hạn tin
  VideoUrl            nvarchar(max)   Link video
  PropertyTypeId      uniqueidentifier FK → PropertyTypes
  AgentId             uniqueidentifier FK → Agents
  TransactionTypeId   uniqueidentifier FK → TransactionTypes (nullable)
  + audit + soft delete + RowVersion
  INDEX: Status, ListingType, IsFeatured, PublishedAt, AgentId, ProvinceCode

TABLE PropertyImages
  Id                  uniqueidentifier PK
  PropertyId          uniqueidentifier FK → Properties (CASCADE delete)
  Url                 nvarchar(2000)  URL ảnh / uploads
  Caption             nvarchar(500)
  DisplayOrder        int             DEFAULT 0
  IsPrimary           bit             Ảnh đại diện
  + audit + soft delete + RowVersion

TABLE PropertyViews (Thống kê lượt xem)
  Id                  uniqueidentifier PK
  PropertyId          uniqueidentifier FK → Properties (CASCADE)
  UserId              nvarchar(450)   Nullable — khách vãng lai
  IpAddress           nvarchar(45)
  UserAgent           nvarchar(500)
  SessionId           nvarchar(200)
  ViewedAt            datetime2

TABLE Favorites (Yêu thích)
  Id                  uniqueidentifier PK
  UserId              nvarchar(450)   FK logic → AspNetUsers
  PropertyId          uniqueidentifier FK → Properties (CASCADE)
  Note                nvarchar(1000)
  + audit + soft delete + RowVersion
  UNIQUE (UserId, PropertyId)

-- ── GIAO DỊCH & THANH TOÁN ─────────────────────────────────────────────────

TABLE Transactions (Giao dịch BĐS)
  Id                  uniqueidentifier PK
  PropertyId          uniqueidentifier FK → Properties
  CustomerId          uniqueidentifier FK → Customers
  AgentId             uniqueidentifier FK → Agents
  TransactionType     int             ListingType enum
  Amount              decimal(18,2)   Giá giao dịch
  Currency            nvarchar(10)
  CommissionAmount    decimal(18,2)   Hoa hồng môi giới
  CommissionCurrency  nvarchar(10)
  Status              int             TransactionStatus
  CompletedAt         datetime2
  Notes               nvarchar(3000)
  ContractNumber      nvarchar(100)   Số hợp đồng
  + audit + soft delete + RowVersion

TABLE UserVIPPackages (Gói VIP của user)
  Id                  uniqueidentifier PK
  UserId              nvarchar(450)
  VIPPackageId        uniqueidentifier FK → VIPPackages
  StartDate           datetime2
  EndDate             datetime2
  RemainingListings   int             Số tin còn lại
  IsActive            bit
  + audit + soft delete + RowVersion
  INDEX (UserId, IsActive)

TABLE Payments (Thanh toán VNPay / gateway)
  Id                  uniqueidentifier PK
  UserId              nvarchar(450)
  UserVIPPackageId    uniqueidentifier FK → UserVIPPackages (SET NULL)
  VIPPackageId        uniqueidentifier FK → VIPPackages (nullable)
  Amount              decimal(18,2)
  Currency            nvarchar(10)
  Status              int             PaymentStatus
  PaymentReference    nvarchar(100)   UNIQUE — mã đơn hàng nội bộ
  TransactionCode     nvarchar(500)   Mã giao dịch gateway
  VnpayTransactionId  nvarchar(200)
  PaymentMethod       nvarchar(100)   VNPay, BankTransfer, …
  BankCode            nvarchar(50)
  OrderInfo           nvarchar(500)
  GatewayResponse     nvarchar(max)   Raw response
  PaidAt              datetime2
  Notes               nvarchar(2000)
  IpAddress           nvarchar(45)
  + audit + soft delete + RowVersion
  INDEX: Status, CreatedAt, UserId

-- ── NHẮN TIN & THÔNG BÁO ───────────────────────────────────────────────────

TABLE Conversations
  Id                  uniqueidentifier PK
  PropertyId          uniqueidentifier FK → Properties
  BuyerId             nvarchar(450)   Người mua/quan tâm
  SellerId            nvarchar(450)   Môi giới / người bán
  IsClosed            bit
  ClosedAt            datetime2
  Subject             nvarchar(500)
  + audit + soft delete + RowVersion

TABLE Messages
  Id                  uniqueidentifier PK
  ConversationId      uniqueidentifier FK → Conversations
  SenderId            nvarchar(450)
  Content             nvarchar(4000)
  Status              int             MessageStatus
  ReadAt              datetime2
  + audit + soft delete + RowVersion

TABLE Notifications
  Id                  uniqueidentifier PK
  UserId              nvarchar(450)
  Title               nvarchar(500)
  Content             nvarchar(2000)
  Type                int             NotificationType
  IsRead              bit
  ReadAt              datetime2
  ReferenceId         nvarchar(450)   Id entity liên quan
  ActionUrl           nvarchar(2000)  Deep link
  + audit + soft delete + RowVersion
  INDEX (UserId, IsRead)

================================================================================
  GHI CHÚ THIẾT KẾ
================================================================================

1. Properties lưu song song 2 mô hình địa chỉ:
   - Address* columns: form nhập tự do / legacy
   - ProvinceCode/WardCode: dropdown chuẩn VN, FK tới Provinces/Wards

2. Soft delete: DELETE API không xóa vật lý; filter IsDeleted = 0 khi query.

3. RowVersion: optimistic concurrency — tránh ghi đè đồng thời.

4. Ảnh upload lưu file tại wwwroot/uploads/properties/{PropertyId}/;
   PropertyImages.Url trỏ tới đường dẫn tương đối hoặc URL ngoài.

5. Script DDL thực thi: scripts/sql/04-full-schema-idempotent.sql
   (sinh tự động từ EF Core migrations, đồng bộ 100% với code C#)

================================================================================
*/

-- File này là tài liệu thiết kế (comment-only).
-- Để tạo schema, chạy:
--   :r 01-create-database.sql
--   :r 04-full-schema-idempotent.sql
GO
