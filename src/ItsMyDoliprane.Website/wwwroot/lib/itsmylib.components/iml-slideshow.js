var __decorate = (this && this.__decorate) || function (decorators, target, key, desc) {
    var c = arguments.length, r = c < 3 ? target : desc === null ? desc = Object.getOwnPropertyDescriptor(target, key) : desc, d;
    if (typeof Reflect === "object" && typeof Reflect.decorate === "function") r = Reflect.decorate(decorators, target, key, desc);
    else for (var i = decorators.length - 1; i >= 0; i--) if (d = decorators[i]) r = (c < 3 ? d(r) : c > 3 ? d(target, key, r) : d(target, key)) || r;
    return c > 3 && r && Object.defineProperty(target, key, r), r;
};
var ImlSlideshow_1;
import { ImlHTMLElement } from './lib/iml-html-element.js';
import { customElement, property } from './lib/decorators.js';
let ImlSlideshow = ImlSlideshow_1 = class ImlSlideshow extends ImlHTMLElement {
    constructor() {
        super(...arguments);
        Object.defineProperty(this, "_indexImage", {
            enumerable: true,
            configurable: true,
            writable: true,
            value: -1
        }); // -1 default image, >= 0 index images slideshow
        Object.defineProperty(this, "_interval", {
            enumerable: true,
            configurable: true,
            writable: true,
            value: void 0
        });
        Object.defineProperty(this, "$image", {
            enumerable: true,
            configurable: true,
            writable: true,
            value: null
        });
        /** Le mode de lecture du diaporama */
        Object.defineProperty(this, "mode", {
            enumerable: true,
            configurable: true,
            writable: true,
            value: 'hover'
        });
        /** L'état de lecture du diaporama */
        Object.defineProperty(this, "status", {
            enumerable: true,
            configurable: true,
            writable: true,
            value: 'active'
        });
        /** L'url par défaut de l'image lorsque le diaporama est à l'arrêt */
        Object.defineProperty(this, "defaultImageUrl", {
            enumerable: true,
            configurable: true,
            writable: true,
            value: void 0
        });
        /** Les urls des images qui défilent lors du diaporama */
        Object.defineProperty(this, "imageUrls", {
            enumerable: true,
            configurable: true,
            writable: true,
            value: void 0
        });
        /** Mode de chargement des images */
        Object.defineProperty(this, "loading", {
            enumerable: true,
            configurable: true,
            writable: true,
            value: 'lazy'
        });
    }
    disconnected() {
        this._clearInterval();
        ImlSlideshow_1._observer.unobserve(this);
    }
    beforeRender() {
        this._clearInterval();
        ImlSlideshow_1._observer.unobserve(this);
    }
    _clearInterval() {
        clearInterval(this._interval);
        this._indexImage = -1;
    }
    html() {
        return `<img src="${this._currentImageUrl ?? ""}" alt="" loading="${this.loading}" />`;
    }
    renderUpdated() {
        this.$image = this.queryShadowSelector('img');
        if (this.status == 'active') {
            if (this.mode == 'hover')
                this._initHover();
            else
                this._startAutoplay();
        }
    }
    _initHover() {
        if (this._isCoarsePointer()) {
            this.$image.addEventListener('load', () => this._loadFirstImage(), { once: true });
            this.addEventListener('touchmove', () => this._startHover());
            ImlSlideshow_1._observer.observe(this); // stopHover
        }
        else {
            this.addEventListener('mouseenter', () => this._startHover());
            this.addEventListener('mouseleave', () => this._stopHover());
        }
        this.$image?.addEventListener('error', () => this._errorImage());
    }
    _isCoarsePointer() {
        return window.matchMedia('(pointer: coarse)').matches;
    }
    _loadFirstImage() {
        if ((this.imageUrls?.length ?? 0) >= 1) {
            const image = new Image();
            image.src = this.imageUrls[0];
        }
    }
    _errorImage() {
        if (this._indexImage != -1) {
            this._clearInterval();
            this._updateImage();
        }
    }
    _startHover() {
        if (this != ImlSlideshow_1._currentHoverImlSlideshow) {
            ImlSlideshow_1._currentHoverImlSlideshow?._stopHover();
            ImlSlideshow_1._currentHoverImlSlideshow = this;
            if (this._isCoarsePointer())
                this._nextImage();
            this._interval = setInterval(() => this._nextImage(), 700);
            this._preloadImages();
        }
    }
    _stopHover() {
        ImlSlideshow_1._currentHoverImlSlideshow = null;
        this._clearInterval();
        this._updateImage();
    }
    _updateImage() {
        const imageUrl = this._currentImageUrl;
        if (imageUrl)
            this.$image.src = imageUrl;
    }
    _startAutoplay() {
        this._interval = setInterval(() => this._nextImage(), 700);
        this._preloadImages();
    }
    _nextImage() {
        if ((this.imageUrls?.length ?? 0) == 0)
            this._indexImage = -1;
        else if (this._indexImage == -1)
            this._indexImage = 0;
        else if (this._indexImage >= (this.imageUrls?.length ?? 0) - 1)
            this._indexImage = 0;
        else
            this._indexImage++;
        this._updateImage();
    }
    get _currentImageUrl() {
        if (this._indexImage == -1)
            return this.defaultImageUrl;
        else if ((this.imageUrls?.length ?? 0) > this._indexImage)
            return this.imageUrls[this._indexImage];
        else
            return undefined;
    }
    _preloadImages() {
        if (this.imageUrls) {
            for (const imageUrl of this.imageUrls) {
                const image = new Image();
                image.src = imageUrl;
            }
        }
    }
    css() {
        // "display: flex;" used because space under image and height increase of 5px
        return `
        <!--suppress CssUnresolvedCustomProperty -->
        <style>
            img {
                display: flex;
                border-radius: var(--iml-slideshow-border-radius, 0);
                width: 100%;
                height: 100%;
                pointer-events: none;
            }
        </style>`;
    }
};
Object.defineProperty(ImlSlideshow, "_currentHoverImlSlideshow", {
    enumerable: true,
    configurable: true,
    writable: true,
    value: null
});
(() => {
    ImlSlideshow_1._observer = new IntersectionObserver((entries) => {
        entries.forEach(entry => {
            const target = entry.target;
            if (!entry.isIntersecting && target == ImlSlideshow_1._currentHoverImlSlideshow)
                target._stopHover();
        });
    }, { root: null, threshold: 0.5 });
    document.addEventListener('visibilitychange', function () {
        if (document.visibilityState === 'visible')
            ImlSlideshow_1._currentHoverImlSlideshow?._stopHover();
    });
})();
__decorate([
    property({ render: true })
], ImlSlideshow.prototype, "mode", void 0);
__decorate([
    property({ render: true })
], ImlSlideshow.prototype, "status", void 0);
__decorate([
    property({ render: true })
], ImlSlideshow.prototype, "defaultImageUrl", void 0);
__decorate([
    property({ type: 'object' })
], ImlSlideshow.prototype, "imageUrls", void 0);
__decorate([
    property({ render: true })
], ImlSlideshow.prototype, "loading", void 0);
ImlSlideshow = ImlSlideshow_1 = __decorate([
    customElement('iml-slideshow')
], ImlSlideshow);
export { ImlSlideshow };
