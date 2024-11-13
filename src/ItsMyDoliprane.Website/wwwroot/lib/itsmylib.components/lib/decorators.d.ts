import { ImlHTMLElement } from './iml-html-element.js';
type PropertyType = 'string' | 'object' | 'number';
interface PropertyOptions {
    /** Indicateur de mise à jour du rendu si la propriété change de valeur, par défaut 'false' */
    render?: boolean;
    /** Le type de la propriété pour convertir la valeur de l'attribut vers le bon type, par défaut 'string' */
    type?: PropertyType;
}
/**
 * Décorateur de classe pour définir le nom de la balise du tag du composant personnalisé à la classe
 *
 * @param tag Le nom de la balise du tag
 */
export declare function customElement(tag: string): <T extends {
    new (...params: any[]): ImlHTMLElement;
}>(constr: T) => void;
/**
 * Décorateur de propriété pour associer l'attribut du même nom à la propriété
 *
 * @param {PropertyOptions|null} options Les options de la propriété
 */
export declare function property(options?: PropertyOptions | null): <T extends ImlHTMLElement>(target: T, propertyKey: string) => void;
export {};
