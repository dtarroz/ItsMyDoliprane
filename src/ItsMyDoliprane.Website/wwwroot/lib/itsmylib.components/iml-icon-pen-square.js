var __decorate = (this && this.__decorate) || function (decorators, target, key, desc) {
    var c = arguments.length, r = c < 3 ? target : desc === null ? desc = Object.getOwnPropertyDescriptor(target, key) : desc, d;
    if (typeof Reflect === "object" && typeof Reflect.decorate === "function") r = Reflect.decorate(decorators, target, key, desc);
    else for (var i = decorators.length - 1; i >= 0; i--) if (d = decorators[i]) r = (c < 3 ? d(r) : c > 3 ? d(target, key, r) : d(target, key)) || r;
    return c > 3 && r && Object.defineProperty(target, key, r), r;
};
import { ImlIconElement } from './lib/iml-icon-element.js';
import { customElement } from './lib/decorators.js';
let ImlIconPenSquare = class ImlIconPenSquare extends ImlIconElement {
    html() {
        return `<svg viewBox="0 0 512 512">
                    <g transform="translate(0,512) scale(0.1,-0.1)">
                        <path d="M4485 5100 c-73 -15 -154 -51 -212 -95 -32 -23 -125 -111 -208 -194 l-150 -151 378 -377 377 -378 160 160 c89 88 177 185 198 215 142 214 115 489 -67 671 -127 127 -305 183 -476 149z"/>
                        <path d="M2833 3578 c-685 -686 -853 -860 -861 -888 -5 -19 -42 -201 -82 -405 -49 -246 -71 -379 -67 -398 9 -35 39 -65 74 -74 38 -9 801 143 840 167 15 10 406 396 868 859 l840 841 -375 375 c-206 206 -377 375 -380 375 -3 0 -388 -384 -857 -852z"/>
                        <path d="M523 4245 c-192 -35 -370 -171 -457 -350 -70 -144 -67 -43 -64 -1805 l3 -1585 22 -65 c33 -96 90 -186 163 -260 75 -75 149 -121 247 -155 l68 -25 1622 0 1622 0 78 25 c201 63 359 231 420 446 17 60 18 125 18 995 l0 931 -30 48 c-44 71 -118 109 -198 102 -70 -6 -125 -39 -164 -99 l-28 -42 -5 -920 c-5 -842 -6 -924 -22 -952 -24 -44 -81 -91 -125 -104 -27 -8 -490 -10 -1584 -8 l-1546 3 -39 27 c-21 15 -50 44 -64 65 l-25 37 -3 1558 c-2 1519 -2 1559 17 1596 11 20 32 49 48 64 63 61 19 58 989 58 673 0 897 3 928 12 57 17 119 82 135 141 27 95 -13 195 -97 244 l-47 28 -905 2 c-712 1 -920 -1 -977 -12z"/>
                    </g>
                </svg>`;
    }
};
ImlIconPenSquare = __decorate([
    customElement('iml-icon-pen-square')
], ImlIconPenSquare);
export { ImlIconPenSquare };
