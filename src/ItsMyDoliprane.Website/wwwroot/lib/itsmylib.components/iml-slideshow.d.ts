import { ImlHTMLElement } from './lib/iml-html-element.js';
export declare class ImlSlideshow extends ImlHTMLElement {
    private static _currentHoverImlSlideshow;
    private static _observer;
    private _indexImage;
    private _interval;
    private $image;
    /** Le mode de lecture du diaporama */
    mode: 'hover' | 'autoplay';
    /** L'état de lecture du diaporama */
    status: 'active' | 'inactive';
    /** L'url par défaut de l'image lorsque le diaporama est à l'arrêt */
    defaultImageUrl?: string;
    /** Les urls des images qui défilent lors du diaporama */
    imageUrls?: string[];
    /** Mode de chargement des images */
    loading: 'lazy' | 'eager';
    protected disconnected(): void;
    protected beforeRender(): void;
    private _clearInterval;
    protected html(): string;
    protected renderUpdated(): void;
    private _initHover;
    private _isCoarsePointer;
    private _loadFirstImage;
    private _errorImage;
    private _startHover;
    private _stopHover;
    private _updateImage;
    private _startAutoplay;
    private _nextImage;
    private get _currentImageUrl();
    private _preloadImages;
    protected css(): string;
}
