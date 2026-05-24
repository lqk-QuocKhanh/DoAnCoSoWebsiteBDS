window.authStorage = {
  get: function () {
    return localStorage.getItem('vpe_auth') || sessionStorage.getItem('vpe_auth');
  },
  set: function (value, rememberMe) {
    if (rememberMe) {
      localStorage.setItem('vpe_auth', value);
      sessionStorage.removeItem('vpe_auth');
    } else {
      sessionStorage.setItem('vpe_auth', value);
      localStorage.removeItem('vpe_auth');
    }
  },
  clear: function () {
    localStorage.removeItem('vpe_auth');
    sessionStorage.removeItem('vpe_auth');
  },
  useRememberMe: function () {
    return !!localStorage.getItem('vpe_auth');
  }
};
