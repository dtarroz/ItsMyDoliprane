import { ImlHTMLElement } from './iml-html-element.js';
const allPropertiesAttributesByClassName = new Map();
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
        // "class extends" used because the real properties are created in the class and not in the parent class,
        // so I wrap the class to create them after the constructor
        const newClassImlHTMLElement = class extends constr {
            constructor(...args) {
                super(...args);
                const that = this;
                const propertiesAttributes = allPropertiesAttributesByClassName.get(className);
                if (propertiesAttributes) {
                    for (const propertyAttribute of propertiesAttributes) {
                        that[`_${propertyAttribute.property}`] = this.convertAttribute(propertyAttribute)
                            ?? that[propertyAttribute.property];
                        Object.defineProperty(this, propertyAttribute.property, {
                            get() {
                                return this[`_${propertyAttribute.property}`];
                            },
                            set(value) {
                                this[`_${propertyAttribute.property}`] = value;
                                if (propertyAttribute.render)
                                    this.render();
                            },
                            enumerable: true,
                            configurable: true
                        });
                    }
                }
                this.render();
            }
            convertAttribute(propertyAttribute) {
                const value = this.getAttribute(propertyAttribute.attribute);
                if (value) {
                    if (propertyAttribute.type == 'object')
                        return JSON.parse(value);
                    if (propertyAttribute.type == 'number')
                        return Number(value);
                }
                return value; // value is null, undefined or string
            }
            ;
        };
        // "defineProperty" used because the constructor name was the name of the variable that contains the overriding class
        Object.defineProperty(newClassImlHTMLElement, 'name', { value: className });
        customElements.define(tag, newClassImlHTMLElement);
    };
}
/**
 * Décorateur de propriété pour associer l'attribut du même nom à la propriété
 *
 * @param {PropertyOptions|null} options Les options de la propriété
 */
export function property(options = null) {
    return (target, propertyKey) => {
        if (typeof target[propertyKey] === 'function')
            throw new Error(`Illegal decorator '@property' on '${propertyKey}', only on property`);
        const className = target.constructor.name;
        const propertyAttribute = {
            property: propertyKey,
            attribute: toKebabCase(propertyKey),
            render: options?.render ?? false,
            type: options?.type ?? 'string'
        };
        if (!allPropertiesAttributesByClassName.has(className))
            allPropertiesAttributesByClassName.set(className, [propertyAttribute]);
        else
            allPropertiesAttributesByClassName.get(className).push(propertyAttribute);
    };
}
function toKebabCase(text) {
    return text.replace(/([a-z])([A-Z])/g, '$1-$2').toLowerCase();
}
