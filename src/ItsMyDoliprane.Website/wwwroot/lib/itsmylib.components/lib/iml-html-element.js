/**
 * Classe de base pour tous les éléments HTML personnalisés et son paramètre générique T représente tous les types de CustomEvent
 * que l'on peut distribuer sur cet élément.
 */
export class ImlHTMLElement extends HTMLElement {
    constructor() {
        super();
        this.attachShadow({ mode: 'open' });
    }
    /**
     * Rendu HTML du Shadow DOM
     */
    html() {
        return '';
    }
    /**
     * Style CSS dans le Shadow DOM.
     * Ne pas oublier d'inclure la balise <style></style>.
     */
    css() {
        return '';
    }
    // noinspection JSUnusedGlobalSymbols
    render() {
        this.beforeRender();
        this.renderHtml();
        this.renderUpdated();
    }
    /**
     * Se produit avant chaque mise à jour du rendu HTML du Shadow DOM
     */
    beforeRender() {
    }
    renderHtml() {
        this.shadowRoot.innerHTML = `${this.css()}${this.html()}`;
    }
    /**
     * Se produit à chaque mise à jour d'une propriété qui provoque une mise à jour du rendu HTML du Shadow DOM
     */
    renderUpdated() {
    }
    /**
     * Renvoie le premier élément descendant du nœud qui correspond aux sélecteurs
     */
    queryShadowSelector(selectors) {
        return this.shadowRoot.querySelector(selectors);
    }
    /**
     * Distribue un événement à la cible et renvoie true si la valeur de l'attribut cancelable est false ou si sa méthode PreventDefault()
     * n'a pas été invoquée, et false dans le cas contraire.
     *
     * @param type Le type d'évènement à distribuer
     * @param options Les caractéristiques de l'évènement
     */
    dispatchCustomEvent(type, options = undefined) {
        return this.dispatchEvent(new CustomEvent(type, { bubbles: true, ...options }));
    }
    /**
     * Attache une fonction à appeler chaque fois que l'évènement spécifié est envoyé à la cible.
     *
     * @param type Le type d'évènement à écouter
     * @param listener La fonction qui est appelée lorsqu'un évènement du type spécifié se produit
     * @param options Les caractéristiques de l'écouteur d'évènements
     */
    addEventListener(type, listener, options) {
        super.addEventListener(type, listener, options);
    }
    // noinspection JSUnusedGlobalSymbols
    disconnectedCallback() {
        this.disconnected();
    }
    /**
     * Se produit lorsque l'élement HTML personnalisé est retiré du document
     */
    disconnected() {
    }
}
