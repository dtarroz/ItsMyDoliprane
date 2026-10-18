import { ImlHTMLElement } from './lib/iml-html-element.js';
import './iml-icon-cross.js';
type TypeCustomEventImlDialog = 'iml-dialog:show';
export declare class ImlDialog extends ImlHTMLElement<TypeCustomEventImlDialog> {
    private $container;
    private $buttonClose;
    private $title;
    /** Le titre de la boite de dialogue modale */
    headerTitle?: string;
    /** Affiche ou non le bouton de fermeture */
    closeButton: boolean;
    /** L'état du rendu de la boite de dialogue */
    size: 'x-small' | 'small' | 'large';
    /** Affiche la boite de dialogue modale */
    show(): void;
    /** Ferme la boite de dialogue modale */
    close(): void;
    protected html(): string;
    protected renderUpdated(): void;
    private _updateAttribut;
    private isShowing;
    protected css(): string;
}
export {};
