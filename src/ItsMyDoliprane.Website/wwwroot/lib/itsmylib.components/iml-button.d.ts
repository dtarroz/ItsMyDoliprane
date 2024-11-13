import { ImlHTMLElement } from './lib/iml-html-element.js';
type TypeCustomEventImlButton = 'iml-button:click';
export declare class ImlButton extends ImlHTMLElement<TypeCustomEventImlButton> {
    private $button;
    /** Le mode de rendu du bouton */
    mode: 'primary' | 'secondary';
    /** L'url de redirection après le clic sur le bouton, seulement si l'événement n'a pas été explicitement annulé */
    redirectToUrl?: string;
    /** L'état du rendu du bouton */
    status: 'active' | 'inactive' | 'disabled';
    /** Déclenche l'événement click sur le bouton */
    click(): void;
    protected html(): string;
    protected renderUpdated(): void;
    protected css(): string;
}
export {};
