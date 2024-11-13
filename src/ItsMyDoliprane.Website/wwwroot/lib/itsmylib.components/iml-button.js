var __decorate = (this && this.__decorate) || function (decorators, target, key, desc) {
    var c = arguments.length, r = c < 3 ? target : desc === null ? desc = Object.getOwnPropertyDescriptor(target, key) : desc, d;
    if (typeof Reflect === "object" && typeof Reflect.decorate === "function") r = Reflect.decorate(decorators, target, key, desc);
    else for (var i = decorators.length - 1; i >= 0; i--) if (d = decorators[i]) r = (c < 3 ? d(r) : c > 3 ? d(target, key, r) : d(target, key)) || r;
    return c > 3 && r && Object.defineProperty(target, key, r), r;
};
import { ImlHTMLElement } from './lib/iml-html-element.js';
import { customElement, property } from './lib/decorators.js';
let ImlButton = class ImlButton extends ImlHTMLElement {
    constructor() {
        super(...arguments);
        Object.defineProperty(this, "$button", {
            enumerable: true,
            configurable: true,
            writable: true,
            value: null
        });
        /** Le mode de rendu du bouton */
        Object.defineProperty(this, "mode", {
            enumerable: true,
            configurable: true,
            writable: true,
            value: 'primary'
        });
        /** L'url de redirection après le clic sur le bouton, seulement si l'événement n'a pas été explicitement annulé */
        Object.defineProperty(this, "redirectToUrl", {
            enumerable: true,
            configurable: true,
            writable: true,
            value: void 0
        });
        /** L'état du rendu du bouton */
        Object.defineProperty(this, "status", {
            enumerable: true,
            configurable: true,
            writable: true,
            value: 'active'
        });
    }
    /** Déclenche l'événement click sur le bouton */
    click() {
        this.$button?.click();
    }
    html() {
        return `<button class="${this.mode} ${this.status}"><slot></slot></button>`;
    }
    renderUpdated() {
        this.$button = this.queryShadowSelector('button');
        this.$button.addEventListener('click', (event) => {
            if (this.status == 'active') {
                if (!this.dispatchCustomEvent('iml-button:click', { cancelable: true }))
                    event.preventDefault();
                else if (this.redirectToUrl)
                    document.location = this.redirectToUrl;
            }
        });
    }
    css() {
        return `
        <!--suppress CssUnresolvedCustomProperty -->
        <style>
            :host {
                width: 150px;
            }
            
            button {
                padding: 12px;
                border-radius: 7px;
                border: 0;
                width: 100%;
                font-family: Arial, sans-serif;
                font-weight: bold;
                font-size: var(--iml-button-font-size, 0.91rem);
            }
            
            .primary {
                color: #161616;
                background-color: hsl(60, 94%, 42%);
            }
            
            .primary.active:hover {
                background-color: hsl(60, 94%, 47%);
            }
            
            .secondary {
                color:  #c6c6c6;
                background-color: transparent;
                border: 1px solid  #c6c6c6;
            }
            
            .secondary.active:hover {
                border: 1px solid hsl(60, 94%, 47%);
            }
            
            .active:hover {
                cursor: pointer;
            }
            
            .disabled {
                opacity: 0.5;
                cursor: default;
            }
        </style>`;
    }
};
__decorate([
    property({ render: true })
], ImlButton.prototype, "mode", void 0);
__decorate([
    property()
], ImlButton.prototype, "redirectToUrl", void 0);
__decorate([
    property({ render: true })
], ImlButton.prototype, "status", void 0);
ImlButton = __decorate([
    customElement('iml-button')
], ImlButton);
export { ImlButton };
