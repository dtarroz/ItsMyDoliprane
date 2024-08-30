export class ButtonTest extends HTMLElement {
    constructor() {
        super();
        this.innerHTML='<button>Testeur</button>';
    }
}

customElements.define('button-test', ButtonTest);