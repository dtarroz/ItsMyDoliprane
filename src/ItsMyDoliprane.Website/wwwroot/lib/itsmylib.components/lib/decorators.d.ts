import { ImlHTMLElement } from './iml-htmlelement.js';
/**
 * Décorateur de classe pour définir le nom de la balise du tag du composant personnalisé à la classe
 *
 * @param tag Le nom de la balise du tag
 */
export declare function customElement(tag: string): <T extends {
    new (...params: any[]): ImlHTMLElement<string>;
}>(constr: T) => void;
/**
 * Décorateur de propriété pour associer l'attribut du même nom à la propriété
 */
export declare function property(): <T extends ImlHTMLElement<string>>(target: T, propertyKey: string) => void;
