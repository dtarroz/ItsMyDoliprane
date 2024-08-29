export class ButtonTest extends HTMLElement {
    constructor() {
        super();
        this.innerHTML='<button>Tester</button>';
    }
}

customElements.define('button-test', ButtonTest);