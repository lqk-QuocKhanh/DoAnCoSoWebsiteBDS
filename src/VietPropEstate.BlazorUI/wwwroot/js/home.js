window.urbnScrollGallery = (element, delta) => {
    if (element) {
        element.scrollBy({ left: delta, behavior: 'smooth' });
    }
};
