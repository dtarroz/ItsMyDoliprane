var __decorate = (this && this.__decorate) || function (decorators, target, key, desc) {
    var c = arguments.length, r = c < 3 ? target : desc === null ? desc = Object.getOwnPropertyDescriptor(target, key) : desc, d;
    if (typeof Reflect === "object" && typeof Reflect.decorate === "function") r = Reflect.decorate(decorators, target, key, desc);
    else for (var i = decorators.length - 1; i >= 0; i--) if (d = decorators[i]) r = (c < 3 ? d(r) : c > 3 ? d(target, key, r) : d(target, key)) || r;
    return c > 3 && r && Object.defineProperty(target, key, r), r;
};
import { ImlHTMLElement } from './lib/iml-html-element.js';
import { customElement, property } from './lib/decorators.js';
import './iml-icon-three-dots.js';
let ImlMenu = class ImlMenu extends ImlHTMLElement {
    constructor() {
        super(...arguments);
        Object.defineProperty(this, "$button", {
            enumerable: true,
            configurable: true,
            writable: true,
            value: null
        });
        Object.defineProperty(this, "$menu", {
            enumerable: true,
            configurable: true,
            writable: true,
            value: null
        });
        Object.defineProperty(this, "boundDocumentClick", {
            enumerable: true,
            configurable: true,
            writable: true,
            value: void 0
        });
        /** Nom de l'icône affichée dans le bouton d'action */
        Object.defineProperty(this, "icon", {
            enumerable: true,
            configurable: true,
            writable: true,
            value: 'three-dots'
        });
        /** Liste des actions du menu */
        Object.defineProperty(this, "actions", {
            enumerable: true,
            configurable: true,
            writable: true,
            value: []
        });
        Object.defineProperty(this, "handleActionClick", {
            enumerable: true,
            configurable: true,
            writable: true,
            value: (evt) => {
                const target = evt.currentTarget;
                const name = target.getAttribute('data-name') || undefined;
                if (name) {
                    const shouldProceed = this.dispatchCustomEvent('iml-menu:action-click', { cancelable: true, detail: name });
                    // si l'événement n'a pas été preventDefault, on ferme
                    if (shouldProceed)
                        this.close();
                }
            }
        });
    }
    /** Ouvre le menu */
    open() {
        if (this.$menu?.classList.contains('open')) {
            this.close();
        }
        else {
            this.$menu?.classList.add('open');
            this.$button?.classList.add('open');
        }
    }
    /** Ferme le menu */
    close() {
        this.$menu?.classList.remove('open');
        this.$button?.classList.remove('open');
    }
    html() {
        const itemsHtml = (this.actions || []).map(a => {
            return `
                <div class="item" data-name="${a.name}">
                    <iml-icon name="${a.icon}"></iml-icon>
                    <span class="caption">${a.caption}</span>
                </div>`;
        }).join('');
        return `
            <button class="action">
                <iml-icon name="${this.icon}"></iml-icon>
            </button>
            <div class="menu">
                ${itemsHtml}
            </div>
        `;
    }
    renderUpdated() {
        this.$button = this.queryShadowSelector('button.action');
        this.$menu = this.queryShadowSelector('div.menu');
        this.$button.addEventListener('click', () => this.open());
        // créer listeners pour les items
        const items = Array.from(this.$menu.querySelectorAll('.item'));
        for (const item of items)
            item.addEventListener('click', this.handleActionClick);
        // document click pour fermer si click en dehors
        if (!this.boundDocumentClick) {
            this.boundDocumentClick = (evt) => {
                const path = evt.composedPath ? evt.composedPath() : evt.path || [];
                if (!path.includes(this)) {
                    if (this.$menu?.classList.contains('open'))
                        this.close();
                }
            };
            document.addEventListener('click', this.boundDocumentClick);
        }
    }
    css() {
        return `
        <!--suppress CssUnresolvedCustomProperty -->
        <style>
            :host {
                position: relative;
                display: inline-block;
            }

            button.action {
                padding: 10px 10px;
                border: 0;
                background: transparent;
                cursor: pointer;
                font-family: Arial, sans-serif;
            }
            
            button.action iml-icon {
                fill:  #c6c6c6;
                display: block;
                width: var(--iml-menu-button-action-image-size, 1rem);
                height: var(--iml-menu-button-action-image-size, 1rem);
            }
            
            button.action iml-icon:hover,
            button.action.open iml-icon {
                fill: hsl(60, 94%, 42%);
            }

            .label {
                font-size: 0.95rem;
            }

            .menu {
                position: absolute;
                bottom: calc(100% + 8px);
                right: 0;
                min-width: 160px;
                background: white;
                border: 1px solid #e5e5e5;
                border-radius: 6px;
                box-shadow: 0 8px 24px rgba(0,0,0,0.08);
                padding: 6px 0;
                display: none;
                z-index: 9999;
            }

            .menu.open {
                display: block;
            }

            .item {
                display: flex;
                align-items: center;
                gap: 8px;
                padding: 8px 12px;
                cursor: pointer;
                outline: none;
            }

            .item:hover, .item:focus {
                background: #f5f5f5;
            }

            .item iml-icon {
                width: var(--iml-menu-button-item-image-size, 1rem);
                height: var(--iml-menu-button-item-image-size, 1rem);
                display: inline-block;
            }

            .caption {
                font-size: var(--iml-menu-button-item-font-size, 0.95rem);
            }
        </style>`;
    }
};
__decorate([
    property({ render: true })
], ImlMenu.prototype, "icon", void 0);
__decorate([
    property({ type: 'object', render: true })
], ImlMenu.prototype, "actions", void 0);
ImlMenu = __decorate([
    customElement('iml-menu')
], ImlMenu);
export { ImlMenu };
