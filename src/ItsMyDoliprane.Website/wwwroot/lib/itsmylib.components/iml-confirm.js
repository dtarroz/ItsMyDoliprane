var __decorate = (this && this.__decorate) || function (decorators, target, key, desc) {
    var c = arguments.length, r = c < 3 ? target : desc === null ? desc = Object.getOwnPropertyDescriptor(target, key) : desc, d;
    if (typeof Reflect === "object" && typeof Reflect.decorate === "function") r = Reflect.decorate(decorators, target, key, desc);
    else for (var i = decorators.length - 1; i >= 0; i--) if (d = decorators[i]) r = (c < 3 ? d(r) : c > 3 ? d(target, key, r) : d(target, key)) || r;
    return c > 3 && r && Object.defineProperty(target, key, r), r;
};
import { ImlHTMLElement } from './lib/iml-html-element.js';
import { customElement, property } from './lib/decorators.js';
import './iml-dialog.js';
import './iml-button.js';
let ImlConfirm = class ImlConfirm extends ImlHTMLElement {
    constructor() {
        super(...arguments);
        Object.defineProperty(this, "$dialog", {
            enumerable: true,
            configurable: true,
            writable: true,
            value: null
        });
        Object.defineProperty(this, "$buttonYes", {
            enumerable: true,
            configurable: true,
            writable: true,
            value: null
        });
        Object.defineProperty(this, "$buttonNo", {
            enumerable: true,
            configurable: true,
            writable: true,
            value: null
        });
        /** Le libelle du bouton 'Oui' (celui de gauche) */
        Object.defineProperty(this, "buttonYesCaption", {
            enumerable: true,
            configurable: true,
            writable: true,
            value: 'Oui'
        });
        /** Le mode de rendu du bouton 'Oui' (celui de gauche) */
        Object.defineProperty(this, "buttonYesMode", {
            enumerable: true,
            configurable: true,
            writable: true,
            value: 'primary'
        });
        /** Le libelle du bouton 'Non' (celui de droite) */
        Object.defineProperty(this, "buttonNoCaption", {
            enumerable: true,
            configurable: true,
            writable: true,
            value: 'Non'
        });
        /** Le mode de rendu du bouton 'Non' (celui de droite) */
        Object.defineProperty(this, "buttonNoMode", {
            enumerable: true,
            configurable: true,
            writable: true,
            value: 'secondary'
        });
        /** Le titre de la boite de dialogue modale */
        Object.defineProperty(this, "headerTitle", {
            enumerable: true,
            configurable: true,
            writable: true,
            value: void 0
        });
        /** L'état du rendu de la boite de dialogue */
        Object.defineProperty(this, "size", {
            enumerable: true,
            configurable: true,
            writable: true,
            value: 'small'
        });
    }
    /** Affiche la boite de dialogue modale */
    show() {
        this.$dialog?.show();
    }
    html() {
        return `
            <iml-dialog close-button="false" size="${this.size}" header-title="${this.headerTitle}" data-dialog>
                <slot></slot>
                <div class="footer">
                    <iml-button mode="${this.buttonYesMode}" data-button-yes>
                        ${this.buttonYesCaption}
                    </iml-button>
                    <iml-button mode="${this.buttonNoMode}" data-button-no>
                        ${this.buttonNoCaption}
                    </iml-button>
                </div>
            </iml-dialog>`;
    }
    renderUpdated() {
        this.$dialog = this.queryShadowSelector('[data-dialog]');
        this.$buttonYes = this.queryShadowSelector('[data-button-yes]');
        this.$buttonNo = this.queryShadowSelector('[data-button-no]');
        this.$buttonYes?.addEventListener('iml-button:click', () => this._click('yes'));
        this.$buttonNo?.addEventListener('iml-button:click', () => this._click('no'));
    }
    // @ts-ignore
    _updateAttribut(_, __, ___) {
        if (this.$dialog)
            this.$dialog.headerTitle = this.headerTitle;
    }
    _click(button) {
        this.dispatchCustomEvent('iml-confirm:confirm', { detail: button });
        this.$dialog?.close();
    }
    css() {
        return `
            <style>
                :host {
                    font-family: Arial, sans-serif;
                }
            
                .footer {
                    display: flex;
                    align-items: center;
                    justify-content: center;
                    gap: 50px;
                    padding-top: 10px;
                    padding-bottom: 20px;
                }
            </style>`;
    }
};
__decorate([
    property()
], ImlConfirm.prototype, "buttonYesCaption", void 0);
__decorate([
    property()
], ImlConfirm.prototype, "buttonYesMode", void 0);
__decorate([
    property()
], ImlConfirm.prototype, "buttonNoCaption", void 0);
__decorate([
    property()
], ImlConfirm.prototype, "buttonNoMode", void 0);
__decorate([
    property({ changedCallback: '_updateAttribut' })
], ImlConfirm.prototype, "headerTitle", void 0);
__decorate([
    property()
], ImlConfirm.prototype, "size", void 0);
ImlConfirm = __decorate([
    customElement('iml-confirm')
], ImlConfirm);
export { ImlConfirm };
