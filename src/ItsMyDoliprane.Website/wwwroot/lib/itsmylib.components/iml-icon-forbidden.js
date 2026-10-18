var __decorate = (this && this.__decorate) || function (decorators, target, key, desc) {
    var c = arguments.length, r = c < 3 ? target : desc === null ? desc = Object.getOwnPropertyDescriptor(target, key) : desc, d;
    if (typeof Reflect === "object" && typeof Reflect.decorate === "function") r = Reflect.decorate(decorators, target, key, desc);
    else for (var i = decorators.length - 1; i >= 0; i--) if (d = decorators[i]) r = (c < 3 ? d(r) : c > 3 ? d(target, key, r) : d(target, key)) || r;
    return c > 3 && r && Object.defineProperty(target, key, r), r;
};
import { ImlIconElement } from './lib/iml-icon-element.js';
import { customElement } from './lib/decorators.js';
let ImlIconForbidden = class ImlIconForbidden extends ImlIconElement {
    html() {
        return `<svg viewBox="0 0 512 512">
                    <g transform="translate(0,512) scale(0.1,-0.1)">
                        <path d="M2370 5114 c-19 -2 -78 -9 -130 -15 -548 -62 -1083 -325 -1489 -730 -405 -406 -652 -907 -732 -1489 -18 -125 -18 -515 0 -640 80 -582 327 -1083 732 -1489 658 -657 1607 -906 2504 -654 929 260 1646 1043 1819 1987 156 845 -103 1682 -705 2285 -399 398 -879 639 -1449 726 -100 15 -472 28 -550 19z m442 -209 c977 -103 1791 -808 2033 -1759 54 -215 69 -342 69 -586 0 -244 -15 -371 -69 -586 -212 -831 -872 -1491 -1700 -1699 -216 -54 -342 -69 -585 -69 -242 0 -368 15 -585 69 -724 182 -1333 718 -1609 1415 -281 713 -199 1524 218 2160 488 744 1344 1149 2228 1055z"/>
                        <path d="M2345 4630 c-715 -81 -1334 -514 -1650 -1155 -285 -578 -286 -1249 0 -1830 71 -146 166 -295 229 -361 45 -46 76 -55 119 -33 31 16 2805 2788 2823 2821 36 69 1 116 -167 227 -237 156 -504 261 -789 311 -142 24 -433 35 -565 20z m413 -201 c208 -23 408 -78 592 -165 110 -52 270 -143 270 -154 0 -9 -2609 -2614 -2613 -2610 -23 26 -99 162 -146 261 -410 861 -100 1897 719 2402 344 212 779 310 1178 266z"/>
                        <path d="M4075 3869 c-31 -17 -2804 -2791 -2821 -2821 -25 -48 -18 -78 29 -122 116 -111 400 -266 623 -339 435 -142 873 -142 1306 0 524 171 966 557 1213 1058 285 576 285 1255 1 1830 -73 147 -168 296 -231 361 -44 46 -78 55 -120 33z m87 -326 c403 -664 363 -1490 -101 -2112 -389 -521 -1053 -811 -1699 -740 -124 13 -301 52 -411 91 -138 48 -365 161 -451 225 -4 4 2601 2613 2610 2613 3 0 26 -35 52 -77z"/>
                    </g>
                </svg>`;
    }
};
ImlIconForbidden = __decorate([
    customElement('iml-icon-forbidden')
], ImlIconForbidden);
export { ImlIconForbidden };
