export class ButtonTest extends HTMLElement {
    constructor() {
        super();
        this.attachShadow({ mode: 'open' });
        this.shadowRoot!.innerHTML = `${this.styles()}<button>Testeur ${Date.now()}</button>`;
        this.shadowRoot!.querySelector('button')!.addEventListener('click', () => {
            document.location.href = document.location.href;
        });
    }

    private styles() {
        return `
        <style>
            button {
                padding: 10px;
                color: var(--background-color-default);
                border-radius: 7px;
                background-color: var(--button-color-default);
                font-weight: bold;
                font-size: 0.91rem;
                cursor: pointer;
                width: 126px;
                border: none;
                text-align: center;
            }
            
            button:hover {
                background-color: var(--button-color-hover-default);
            }
        </style>
        `;
    }
}

customElements.define('button-test', ButtonTest);