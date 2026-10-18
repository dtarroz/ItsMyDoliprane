var __decorate = (this && this.__decorate) || function (decorators, target, key, desc) {
    var c = arguments.length, r = c < 3 ? target : desc === null ? desc = Object.getOwnPropertyDescriptor(target, key) : desc, d;
    if (typeof Reflect === "object" && typeof Reflect.decorate === "function") r = Reflect.decorate(decorators, target, key, desc);
    else for (var i = decorators.length - 1; i >= 0; i--) if (d = decorators[i]) r = (c < 3 ? d(r) : c > 3 ? d(target, key, r) : d(target, key)) || r;
    return c > 3 && r && Object.defineProperty(target, key, r), r;
};
import { ImlIconElement } from './lib/iml-icon-element.js';
import { customElement } from './lib/decorators.js';
let ImlIconChannel = class ImlIconChannel extends ImlIconElement {
    html() {
        return `<svg viewBox="0 0 512 512">
                    <g transform="matrix(0.1118017,0,0,-0.1118017,-30.31393,542.18647)">
                        <path d="m 1635,4827 c -69,-39 -101,-123 -75,-194 6,-15 147,-164 312,-330 l 302,-302 -625,-4 c -701,-4 -665,0 -834,-82 -196,-95 -332,-251 -408,-470 l -32,-90 -3,-1193 C 270,1005 270,966 289,886 360,587 586,361 884,290 c 78,-19 130,-20 1676,-20 1553,0 1597,1 1677,20 298,71 523,297 594,596 19,80 19,119 17,1276 l -3,1193 -23,70 c -92,273 -287,466 -552,546 -74,23 -83,23 -699,27 l -625,3 306,307 c 297,297 307,308 313,354 4,25 4,57 0,70 -28,87 -128,139 -212,108 -15,-6 -200,-183 -410,-393 l -383,-382 -382,382 c -211,210 -396,387 -411,393 -44,16 -88,12 -132,-13 z M 4165,3665 c 169,-44 296,-163 345,-324 19,-61 20,-98 20,-1205 0,-1248 3,-1182 -57,-1300 -32,-64 -125,-157 -189,-189 -120,-61 -9,-57 -1724,-57 -1715,0 -1604,-4 -1724,57 -64,32 -157,125 -189,189 -60,118 -57,52 -57,1300 0,1107 1,1144 20,1205 49,159 183,285 345,325 39,9 405,12 1600,13 1362,1 1558,-1 1610,-14 z" />
                        <path d="m 2267,2900 c -118,-20 -226,-104 -280,-218 l -32,-67 -3,-459 c -3,-437 -2,-462 17,-525 63,-204 288,-318 488,-248 58,20 757,482 813,537 26,25 59,72 76,105 26,55 29,69 29,165 0,96 -2,110 -29,162 -58,115 -82,131 -507,344 -312,156 -400,195 -451,204 -35,5 -65,9 -68,9 -3,-1 -27,-5 -53,-9 z m 428,-487 c 182,-91 338,-172 348,-181 24,-21 21,-64 -5,-89 -48,-43 -690,-463 -708,-463 -11,0 -29,9 -40,20 -19,19 -20,33 -20,434 v 415 l 27,20 c 17,14 33,19 47,15 12,-4 170,-81 351,-171 z" />
                    </g>
                </svg>`;
    }
};
ImlIconChannel = __decorate([
    customElement('iml-icon-channel')
], ImlIconChannel);
export { ImlIconChannel };
