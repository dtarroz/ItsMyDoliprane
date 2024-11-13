var __decorate = (this && this.__decorate) || function (decorators, target, key, desc) {
    var c = arguments.length, r = c < 3 ? target : desc === null ? desc = Object.getOwnPropertyDescriptor(target, key) : desc, d;
    if (typeof Reflect === "object" && typeof Reflect.decorate === "function") r = Reflect.decorate(decorators, target, key, desc);
    else for (var i = decorators.length - 1; i >= 0; i--) if (d = decorators[i]) r = (c < 3 ? d(r) : c > 3 ? d(target, key, r) : d(target, key)) || r;
    return c > 3 && r && Object.defineProperty(target, key, r), r;
};
import { ImlIconElement } from './lib/iml-icon-element.js';
import { customElement } from './lib/decorators.js';
let ImlIconArrow = class ImlIconArrow extends ImlIconElement {
    html() {
        return `<svg viewBox="0 0 512 512">
                    <g transform="matrix(0,0.20025679,0.20025679,0,-256.64735,-276.53287)">
                        <path d="m 2060,3827 c -76,-24 -140,-118 -140,-205 0,-73 21,-97 508,-584 l 477,-478 -487,-487 -487,-488 -7,-50 c -23,-160 117,-288 270,-245 38,10 106,75 614,583 l 570,572 13,58 c 10,46 10,68 0,115 l -13,57 -571,573 c -449,449 -580,574 -607,582 -44,12 -95,11 -140,-3 z" />
                    </g>
                </svg>`;
    }
};
ImlIconArrow = __decorate([
    customElement('iml-icon-arrow')
], ImlIconArrow);
export { ImlIconArrow };
