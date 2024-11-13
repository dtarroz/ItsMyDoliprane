var __decorate = (this && this.__decorate) || function (decorators, target, key, desc) {
    var c = arguments.length, r = c < 3 ? target : desc === null ? desc = Object.getOwnPropertyDescriptor(target, key) : desc, d;
    if (typeof Reflect === "object" && typeof Reflect.decorate === "function") r = Reflect.decorate(decorators, target, key, desc);
    else for (var i = decorators.length - 1; i >= 0; i--) if (d = decorators[i]) r = (c < 3 ? d(r) : c > 3 ? d(target, key, r) : d(target, key)) || r;
    return c > 3 && r && Object.defineProperty(target, key, r), r;
};
import { ImlIconElement } from './lib/iml-icon-element.js';
import { customElement } from './lib/decorators.js';
let ImlIconCross = class ImlIconCross extends ImlIconElement {
    html() {
        return `<svg viewBox="0 0 100 100">
                    <path d="M 16.831613,0 C 12.802299,0.00741737 8.748277,1.5284016 5.5137026,5.0286786 -3.2024611,14.121852 -0.46505154,22.887634 5.9907835,28.120025 5.7765506,28.324798 5.5619485,28.529146 5.3477578,28.733934 L 26.499528,50.002542 5.3477578,71.269232 c 0.2944319,0.28149 0.5880834,0.563609 0.8824994,0.845104 -6.44069162,5.380966 -9.2292576,14.213213 -0.7165493,22.860173 7.0556401,7.635251 18.0028371,5.842061 24.0991051,-0.509679 0.04904,0.05033 0.101607,0.09743 0.153332,0.146175 L 50.316382,73.94873 70.770441,94.512492 c 0.185812,-0.177752 0.37231,-0.354762 0.558107,-0.532493 9.689556,7.938421 28.881202,7.294511 28.643606,-10.898608 0.290266,-4.779853 -1.713088,-8.879888 -4.859401,-11.840307 0.02584,-0.02331 0.04904,-0.04874 0.07471,-0.07232 L 74.13375,50.000603 95.187468,28.832426 c -0.02584,-0.02225 -0.04798,-0.04715 -0.07324,-0.07073 3.14574,-2.960341 5.147842,-7.060953 4.85752,-11.840308 C 100.20945,-1.2717236 81.017703,-1.9156253 71.328142,6.0227808 71.14233,5.8450818 70.955832,5.6680187 70.77004,5.4903515 L 50.317837,26.05405 29.765708,5.39178 C 29.716668,5.441052 29.664101,5.488894 29.612375,5.5376372 26.310911,2.0985959 21.591185,-0.00871011 16.831254,1.0066428e-4 Z" />
                </svg>`;
    }
};
ImlIconCross = __decorate([
    customElement('iml-icon-cross')
], ImlIconCross);
export { ImlIconCross };
