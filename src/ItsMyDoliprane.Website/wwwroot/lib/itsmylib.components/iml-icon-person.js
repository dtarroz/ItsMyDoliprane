var __decorate = (this && this.__decorate) || function (decorators, target, key, desc) {
    var c = arguments.length, r = c < 3 ? target : desc === null ? desc = Object.getOwnPropertyDescriptor(target, key) : desc, d;
    if (typeof Reflect === "object" && typeof Reflect.decorate === "function") r = Reflect.decorate(decorators, target, key, desc);
    else for (var i = decorators.length - 1; i >= 0; i--) if (d = decorators[i]) r = (c < 3 ? d(r) : c > 3 ? d(target, key, r) : d(target, key)) || r;
    return c > 3 && r && Object.defineProperty(target, key, r), r;
};
import { ImlIconElement } from './lib/iml-icon-element.js';
import { customElement } from './lib/decorators.js';
let ImlIconPerson = class ImlIconPerson extends ImlIconElement {
    html() {
        return `<svg viewBox="0 0 512 512">
                    <g transform="matrix(0.10914773,0,0,-0.10914783,-23.336899,535.46678)">
                        <path d="m 2379,4895 c -303,-49 -541,-173 -744,-386 -169,-177 -279,-388 -332,-639 -24,-117 -24,-363 0,-480 61,-290 196,-526 410,-718 l 58,-52 -38,-16 C 1640,2565 1486,2482 1385,2415 895,2092 564,1587 458,1000 433,860 420,423 439,359 454,309 492,265 544,237 l 41,-22 h 1975 1975 l 45,25 c 24,14 58,44 75,68 l 30,44 3,211 c 4,251 -12,405 -64,607 -131,513 -456,966 -899,1256 -85,55 -262,150 -333,177 l -43,17 60,51 c 61,50 162,166 218,249 80,118 156,303 190,463 25,116 25,380 -1,497 -56,260 -167,463 -355,651 -191,191 -393,300 -656,354 -111,23 -319,28 -426,10 z m 416,-450 c 285,-83 504,-303 587,-591 20,-68 23,-101 22,-234 0,-142 -2,-162 -28,-240 -50,-150 -99,-233 -205,-344 -110,-114 -248,-195 -406,-237 -113,-29 -299,-30 -410,0 -323,86 -566,340 -630,660 -23,112 -16,294 15,401 75,261 282,481 537,571 158,56 356,62 518,14 z m 64,-2126 c 366,-62 739,-271 980,-550 260,-300 396,-636 426,-1046 l 5,-83 H 2560 c -1361,0 -1710,3 -1710,13 0,60 24,259 40,334 139,665 659,1184 1326,1324 204,42 425,45 643,8 z" />
                    </g>
                </svg>`;
    }
};
ImlIconPerson = __decorate([
    customElement('iml-icon-person')
], ImlIconPerson);
export { ImlIconPerson };
