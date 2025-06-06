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
                    <g transform="translate(0,512) scale(0.1,-0.1)">
                        <path d="M2710 4516 l0 -606 -22 -4 c-13 -2 -77 -9 -143 -16 -501 -49 -995 -249 -1430 -579 -142 -107 -421 -390 -531 -536 -348 -465 -535 -968 -574 -1544 -7 -90 -10 -382 -8 -691 l3 -534 184 424 c201 466 247 561 356 725 358 542 894 941 1530 1138 160 50 400 95 582 110 l53 4 2 -605 3 -604 365 298 c201 165 740 605 1198 978 458 374 832 683 832 687 0 3 -57 53 -127 110 -345 280 -2184 1777 -2225 1812 l-48 39 0 -606z m1124 -696 c443 -360 805 -657 805 -660 0 -3 -365 -303 -812 -667 l-812 -663 -3 440 -2 440 -138 0 c-848 -1 -1640 -340 -2232 -956 -98 -102 -240 -274 -302 -364 l-17 -25 6 40 c25 189 96 446 179 642 294 702 918 1255 1649 1462 238 67 391 90 663 98 l192 6 0 434 c0 271 4 433 10 431 5 -1 371 -298 814 -658z" />
                    </g>
                </svg>`;
    }
};
ImlIconArrow = __decorate([
    customElement('iml-icon-share')
], ImlIconArrow);
export { ImlIconArrow };
