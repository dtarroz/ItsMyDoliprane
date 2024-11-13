var __decorate = (this && this.__decorate) || function (decorators, target, key, desc) {
    var c = arguments.length, r = c < 3 ? target : desc === null ? desc = Object.getOwnPropertyDescriptor(target, key) : desc, d;
    if (typeof Reflect === "object" && typeof Reflect.decorate === "function") r = Reflect.decorate(decorators, target, key, desc);
    else for (var i = decorators.length - 1; i >= 0; i--) if (d = decorators[i]) r = (c < 3 ? d(r) : c > 3 ? d(target, key, r) : d(target, key)) || r;
    return c > 3 && r && Object.defineProperty(target, key, r), r;
};
import { ImlIconElement } from './lib/iml-icon-element.js';
import { customElement } from './lib/decorators.js';
let ImlIconFilter = class ImlIconFilter extends ImlIconElement {
    html() {
        return `<svg viewBox="0 0 512 512">
                    <g transform="translate(0,512) scale(0.1,-0.1)">
                        <path d="M275 5106 c-111 -35 -190 -104 -239 -206 l-31 -65 0 -335 0 -335 27 -58 c15 -32 37 -70 48 -84 11 -14 415 -388 898 -830 700 -641 883 -814 907 -855 l30 -53 5 -1092 c5 -1083 5 -1092 26 -1119 39 -52 71 -69 133 -69 l58 0 455 303 c257 171 475 323 501 350 25 26 59 74 74 107 l28 60 5 730 5 730 30 52 c24 41 202 210 835 789 1049 959 973 887 1012 970 l33 69 0 335 0 335 -31 65 c-40 84 -100 144 -182 183 l-67 32 -2260 2 c-1806 1 -2268 -1 -2300 -11z m4509 -324 c15 -17 16 -48 14 -288 l-3 -269 -870 -797 c-478 -438 -887 -816 -907 -840 -45 -51 -89 -132 -114 -208 -17 -51 -19 -112 -24 -775 l-5 -720 -318 -213 -317 -212 0 908 c0 866 -1 912 -19 985 -22 83 -47 139 -95 206 -17 24 -375 359 -796 745 -421 385 -820 751 -887 814 l-123 114 0 266 c0 233 2 268 17 284 15 17 98 18 2223 18 2133 0 2207 -1 2224 -18z" />
                    </g>
                </svg>`;
    }
};
ImlIconFilter = __decorate([
    customElement('iml-icon-filter')
], ImlIconFilter);
export { ImlIconFilter };
