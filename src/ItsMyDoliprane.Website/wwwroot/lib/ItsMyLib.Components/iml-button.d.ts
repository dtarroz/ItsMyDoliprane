import { ImlHTMLElement } from './lib/iml-htmlelement.js';
type TypeCustomEventImlButton = 'iml-button:click';
export declare class ImlButton extends ImlHTMLElement<TypeCustomEventImlButton> {
    /** Le mode de rendu du bouton */
    mode: 'primary' | 'secondary';
    /** L'url de redirection après le clic sur le bouton, seulement si l'événement n'a pas été explicitement annulé */
    redirectToUrl?: string;
    /** Le bouton est inactif si la valeur est égal à true */
    disabled: boolean;
    protected html(): string;
    private componentClass;
    protected renderUpdated(): void;
    protected css(): string;
}
export {};
