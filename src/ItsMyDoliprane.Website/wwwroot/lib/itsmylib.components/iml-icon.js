var __decorate = (this && this.__decorate) || function (decorators, target, key, desc) {
    var c = arguments.length, r = c < 3 ? target : desc === null ? desc = Object.getOwnPropertyDescriptor(target, key) : desc, d;
    if (typeof Reflect === "object" && typeof Reflect.decorate === "function") r = Reflect.decorate(decorators, target, key, desc);
    else for (var i = decorators.length - 1; i >= 0; i--) if (d = decorators[i]) r = (c < 3 ? d(r) : c > 3 ? d(target, key, r) : d(target, key)) || r;
    return c > 3 && r && Object.defineProperty(target, key, r), r;
};
import { ImlIconElement } from './lib/iml-icon-element.js';
import { customElement, property } from './lib/decorators.js';
let ImlIcon = class ImlIcon extends ImlIconElement {
    constructor() {
        super(...arguments);
        /** Le nom de l'icône */
        Object.defineProperty(this, "name", {
            enumerable: true,
            configurable: true,
            writable: true,
            value: 'arrow'
        });
    }
    html() {
        return `<iml-icon-${this.name}></iml-icon-${this.name}>`;
    }
};
__decorate([
    property({ render: true })
], ImlIcon.prototype, "name", void 0);
ImlIcon = __decorate([
    customElement('iml-icon')
], ImlIcon);
export { ImlIcon };
