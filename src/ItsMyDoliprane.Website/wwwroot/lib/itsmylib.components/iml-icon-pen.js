var __decorate = (this && this.__decorate) || function (decorators, target, key, desc) {
    var c = arguments.length, r = c < 3 ? target : desc === null ? desc = Object.getOwnPropertyDescriptor(target, key) : desc, d;
    if (typeof Reflect === "object" && typeof Reflect.decorate === "function") r = Reflect.decorate(decorators, target, key, desc);
    else for (var i = decorators.length - 1; i >= 0; i--) if (d = decorators[i]) r = (c < 3 ? d(r) : c > 3 ? d(target, key, r) : d(target, key)) || r;
    return c > 3 && r && Object.defineProperty(target, key, r), r;
};
import { ImlIconElement } from './lib/iml-icon-element.js';
import { customElement } from './lib/decorators.js';
let ImlIconPen = class ImlIconPen extends ImlIconElement {
    html() {
        return `<svg viewBox="0 0 512 512">
                    <g transform="translate(0,512) scale(0.1,-0.1)">
                        <path d="M4058 5104 c-112 -27 -174 -68 -316 -208 l-133 -131 581 -580 580 -580 116 115 c185 182 234 277 234 449 0 98 -17 167 -63 256 -45 88 -544 587 -633 633 -113 59 -246 76 -366 46z"/>
                        <path d="M2050 3205 l-1405 -1405 580 -580 580 -580 1405 1405 1405 1405 -580 580 -580 580 -1405 -1405z"/>
                        <path d="M272 802 c-144 -433 -262 -790 -262 -794 0 -3 358 113 796 259 l796 265 -528 529 c-291 291 -531 529 -534 529 -3 0 -123 -354 -268 -788z"/>
                    </g>
                </svg>`;
    }
};
ImlIconPen = __decorate([
    customElement('iml-icon-pen')
], ImlIconPen);
export { ImlIconPen };
