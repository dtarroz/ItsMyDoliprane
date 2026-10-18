var __decorate = (this && this.__decorate) || function (decorators, target, key, desc) {
    var c = arguments.length, r = c < 3 ? target : desc === null ? desc = Object.getOwnPropertyDescriptor(target, key) : desc, d;
    if (typeof Reflect === "object" && typeof Reflect.decorate === "function") r = Reflect.decorate(decorators, target, key, desc);
    else for (var i = decorators.length - 1; i >= 0; i--) if (d = decorators[i]) r = (c < 3 ? d(r) : c > 3 ? d(target, key, r) : d(target, key)) || r;
    return c > 3 && r && Object.defineProperty(target, key, r), r;
};
import { ImlIconElement } from './lib/iml-icon-element.js';
import { customElement } from './lib/decorators.js';
let ImlIconSettings = class ImlIconSettings extends ImlIconElement {
    html() {
        return `<svg viewBox="0 0 512 512" fill-rule="evenodd">
                    <path d="M 209.4 108.2 L 215.9 106.3 L 218.7 44.3 L 293.3 44.3 L 296.1 106.3 L 302.6 108.2
                             L 327.6 118.5 L 333.5 121.8 L 379.3 79.9 L 432.1 132.7 L 390.2 178.5 L 393.5 184.4
                             L 403.8 209.4 L 405.7 215.9 L 467.7 218.7 L 467.7 293.3 L 405.7 296.1 L 403.8 302.6
                             L 393.5 327.6 L 390.2 333.5 L 432.1 379.3 L 379.3 432.1 L 333.5 390.2 L 327.6 393.5
                             L 302.6 403.8 L 296.1 405.7 L 293.3 467.7 L 218.7 467.7 L 215.9 405.7 L 209.4 403.8
                             L 184.4 393.5 L 178.5 390.2 L 132.7 432.1 L 79.9 379.3 L 121.8 333.5 L 118.5 327.6
                             L 108.2 302.6 L 106.3 296.1 L 44.3 293.3 L 44.3 218.7 L 106.3 215.9 L 108.2 209.4
                             L 118.5 184.4 L 121.8 178.5 L 79.9 132.7 L 132.7 79.9 L 178.5 121.8 L 184.4 118.5 Z
                             M 334.0 256.0 A 78 78 0 1 0 178.0 256.0 A 78 78 0 1 0 334.0 256.0 Z" />
                </svg>`;
    }
};
ImlIconSettings = __decorate([
    customElement('iml-icon-settings')
], ImlIconSettings);
export { ImlIconSettings };
