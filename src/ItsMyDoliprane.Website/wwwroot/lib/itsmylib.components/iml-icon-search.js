var __decorate = (this && this.__decorate) || function (decorators, target, key, desc) {
    var c = arguments.length, r = c < 3 ? target : desc === null ? desc = Object.getOwnPropertyDescriptor(target, key) : desc, d;
    if (typeof Reflect === "object" && typeof Reflect.decorate === "function") r = Reflect.decorate(decorators, target, key, desc);
    else for (var i = decorators.length - 1; i >= 0; i--) if (d = decorators[i]) r = (c < 3 ? d(r) : c > 3 ? d(target, key, r) : d(target, key)) || r;
    return c > 3 && r && Object.defineProperty(target, key, r), r;
};
import { ImlIconElement } from './lib/iml-icon-element.js';
import { customElement } from './lib/decorators.js';
let ImlIconSearch = class ImlIconSearch extends ImlIconElement {
    html() {
        return `<svg viewBox="0 0 512 512">
                    <g transform="translate(0,512) scale(0.1,-0.1)">
                        <path d="M1680 5099 c-654 -72 -1204 -457 -1490 -1043 -273 -557 -248 -1226 65 -1756 453 -765 1359 -1109 2200 -834 629 206 1110 747 1244 1399 83 405 25 843 -158 1207 -275 548 -774 913 -1381 1013 -119 19 -368 27 -480 14z m485 -439 c196 -44 348 -107 500 -207 324 -214 541 -534 626 -923 18 -86 22 -137 23 -280 1 -264 -38 -433 -154 -665 -200 -401 -576 -683 -1030 -771 -143 -28 -409 -25 -550 5 -567 123 -996 542 -1126 1099 -31 133 -43 370 -25 507 86 651 596 1164 1246 1254 112 16 385 5 490 -19z" />
                        <path d="M1155 3988 c-11 -5 -45 -39 -75 -74 -157 -181 -250 -434 -250 -678 0 -88 23 -227 52 -316 46 -143 154 -319 248 -404 37 -33 47 -37 78 -32 49 8 77 37 76 84 0 31 -10 50 -67 117 -78 94 -148 227 -179 340 -17 64 -21 109 -21 220 0 124 3 150 27 230 36 117 94 228 166 313 73 87 83 106 75 142 -12 56 -78 85 -130 58z" />
                        <path d="M3440 1929 c-56 -72 -176 -192 -265 -267 l-50 -42 188 -187 c142 -143 187 -194 187 -213 0 -42 27 -128 53 -173 14 -23 239 -254 499 -513 444 -443 477 -473 533 -495 46 -17 80 -23 145 -23 121 0 185 27 271 113 84 84 112 150 113 266 1 100 -20 165 -79 243 -24 31 -238 251 -477 488 -386 385 -440 435 -499 463 -40 20 -88 34 -125 38 l-59 6 -185 183 c-102 101 -188 184 -190 183 -3 0 -30 -31 -60 -70z" />
                    </g>
                </svg>`;
    }
};
ImlIconSearch = __decorate([
    customElement('iml-icon-search')
], ImlIconSearch);
export { ImlIconSearch };
