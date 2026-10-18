import { ImlHTMLElement } from './lib/iml-html-element.js';
import './iml-dialog.js';
import './iml-button.js';
type TypeCustomEventImlConfirm = 'iml-confirm:confirm';
export declare class ImlConfirm extends ImlHTMLElement<TypeCustomEventImlConfirm> {
    private $dialog;
    private $buttonYes;
    private $buttonNo;
    /** Le libelle du bouton 'Oui' (celui de gauche) */
    buttonYesCaption: string;
    /** Le mode de rendu du bouton 'Oui' (celui de gauche) */
    buttonYesMode: 'primary' | 'secondary';
    /** Le libelle du bouton 'Non' (celui de droite) */
    buttonNoCaption: string;
    /** Le mode de rendu du bouton 'Non' (celui de droite) */
    buttonNoMode: 'primary' | 'secondary';
    /** Le titre de la boite de dialogue modale */
    headerTitle?: string;
    /** L'état du rendu de la boite de dialogue */
    size: 'x-small' | 'small' | 'large';
    /** Affiche la boite de dialogue modale */
    show(): void;
    protected html(): string;
    protected renderUpdated(): void;
    private _updateAttribut;
    private _click;
    protected css(): string;
}
export {};
