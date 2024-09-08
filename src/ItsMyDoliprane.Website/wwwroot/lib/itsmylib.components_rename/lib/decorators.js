import { ImlHTMLElement } from './iml-htmlelement.js';
const allPropertiesByClassName = new Map();
/**
 * Décorateur de classe pour définir le nom de la balise du tag du composant personnalisé à la classe
 *
 * @param tag Le nom de la balise du tag
 */
export function customElement(tag) {
    return (constr) => {
        const className = constr.prototype.constructor.name;
        if (!(constr.prototype instanceof ImlHTMLElement))
            throw new Error(`Illegal decorator '@customElement' on '${className}', only on class extends ImlHtmlElement`);
        // "class extends" used because the real properties are created in the class and not in the parent class, so I wrap the class to create them after the constructor
        const newClassImlHTMLElement = class extends constr {
            constructor(...args) {
                super(...args);
                const that = this;
                const properties = allPropertiesByClassName.get(className);
                if (properties) {
                    for (const property of properties) {
                        that[`_${property}`] = this.getAttribute(property) ?? that[property];
                        Object.defineProperty(this, property, {
                            get() {
                                return this[`_${property}`];
                            },
                            set(value) {
                                this[`_${property}`] = value;
                                this.render();
                            },
                            enumerable: true,
                            configurable: true
                        });
                    }
                }
                this.render();
            }
        };
        // "defineProperty" used because the constructor name was the name of the variable that contains the overriding class
        Object.defineProperty(newClassImlHTMLElement, 'name', { value: className });
        customElements.define(tag, newClassImlHTMLElement);
    };
}
/**
 * Décorateur de propriété pour associer l'attribut du même nom à la propriété
 */
export function property() {
    return (target, propertyKey) => {
        if (typeof target[propertyKey] === 'function')
            throw new Error(`Illegal decorator '@property' on '${propertyKey}', only on property`);
        const className = target.constructor.name;
        if (!allPropertiesByClassName.has(className))
            allPropertiesByClassName.set(className, [propertyKey]);
        allPropertiesByClassName.get(className).push(propertyKey);
    };
}
