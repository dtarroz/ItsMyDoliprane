import { ImlHTMLElement } from './lib/iml-html-element.js';
import './iml-icon-three-dots.js';
type TypeCustomEventImlMenu = 'iml-menu:action-click';
interface ImlMenuAction {
    name: string;
    icon: string;
    caption: string;
}
export declare class ImlMenu extends ImlHTMLElement<TypeCustomEventImlMenu> {
    private $button;
    private $menu;
    private boundDocumentClick?;
    /** Nom de l'icône affichée dans le bouton d'action */
    icon: string;
    /** Liste des actions du menu */
    actions: ImlMenuAction[];
    /** Ouvre le menu */
    open(): void;
    /** Ferme le menu */
    close(): void;
    protected html(): string;
    protected renderUpdated(): void;
    private handleActionClick;
    protected css(): string;
}
export {};
