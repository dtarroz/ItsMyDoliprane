var __decorate = (this && this.__decorate) || function (decorators, target, key, desc) {
    var c = arguments.length, r = c < 3 ? target : desc === null ? desc = Object.getOwnPropertyDescriptor(target, key) : desc, d;
    if (typeof Reflect === "object" && typeof Reflect.decorate === "function") r = Reflect.decorate(decorators, target, key, desc);
    else for (var i = decorators.length - 1; i >= 0; i--) if (d = decorators[i]) r = (c < 3 ? d(r) : c > 3 ? d(target, key, r) : d(target, key)) || r;
    return c > 3 && r && Object.defineProperty(target, key, r), r;
};
import { ImlIconElement } from './lib/iml-icon-element.js';
import { customElement } from './lib/decorators.js';
let ImlIconCirclePlus = class ImlIconCirclePlus extends ImlIconElement {
    html() {
        return `<svg viewBox="0 0 512 512">
                    <g transform="translate(0,512) scale(0.1,-0.1)">
                        <path d="M2370 5113 c-371 -35 -653 -114 -961 -269 -406 -203 -782 -548 -1029 -944 -179 -286 -309 -655 -362 -1025 -17 -118 -17 -512 0 -630 42 -295 120 -553 242 -800 137 -280 272 -468 494 -691 221 -220 412 -357 681 -489 188 -92 309 -137 500 -185 500 -126 1002 -102 1490 71 150 53 408 183 540 271 560 374 952 942 1095 1588 33 150 50 291 57 465 15 426 -73 832 -263 1214 -124 250 -263 447 -458 648 -214 222 -430 379 -711 518 -296 146 -572 225 -900 255 -102 9 -333 11 -415 3z m545 -342 c628 -106 1158 -448 1511 -977 179 -267 296 -573 351 -909 24 -153 24 -497 0 -650 -108 -668 -474 -1222 -1042 -1580 -243 -153 -537 -261 -850 -312 -154 -24 -497 -24 -650 1 -657 107 -1198 456 -1557 1006 -168 257 -281 557 -335 885 -24 153 -24 497 0 650 81 497 291 912 636 1255 382 381 862 605 1401 654 108 10 418 -4 535 -23z" />
                        <path d="M2495 3966 c-37 -17 -70 -52 -84 -89 -7 -19 -11 -217 -11 -592 l0 -565 -579 -2 c-568 -3 -580 -3 -607 -24 -53 -39 -69 -71 -69 -134 0 -63 16 -95 69 -134 27 -21 39 -21 606 -24 l580 -2 2 -580 c3 -567 3 -579 24 -606 39 -53 71 -69 134 -69 63 0 95 16 134 69 21 27 21 39 24 606 l2 580 580 2 c567 3 579 3 606 24 53 39 69 71 69 134 0 63 -16 95 -69 134 -27 21 -39 21 -606 24 l-580 2 -2 580 c-3 567 -3 579 -24 606 -11 15 -32 37 -46 47 -34 25 -113 32 -153 13z" />
                    </g>
                </svg>`;
    }
};
ImlIconCirclePlus = __decorate([
    customElement('iml-icon-circle-plus')
], ImlIconCirclePlus);
export { ImlIconCirclePlus };