export class TimeProgressBar extends HTMLElement {

    constructor() {
        super();
        this.innerHTML = `<div tabindex="0"><span class="time-title">${this.caption()}</span>${this.isProgressEnd()
            ? `${this.getEndHtml()}${this.getMedicationHtml()}`
            : `${this.getMedicationHtml()}${this.getProgressBarHtml()}`}</div>`;
    }

    isProgressEnd() {
        return this.current() >= this.max();
    }

    getMedicationHtml() {
        let numberMedication = this.numberMedication();
        let html = '<div class="time-medication">';
        for (let i = 0; i < numberMedication; i++) {
            html += '<svg><use xlink:href="#doliprane"></use></svg>';
        }
        html += '</div>';
        return html;
    }

    getEndHtml() {
        let html = '<span class="time-title-end">✅</span>';
        if (this.tooltip())
            html += `<span class="tooltip">${this.tooltip()}</span>`;
        return html;
    }

    getProgressBarHtml() {
        return `<span class="tooltip">${this.tooltip()}</span>
                <div class="time-progress" style = "width: ${this.getProgressWidth()}">
                    <div class="time-progress-bar" 
                         style="width: ${this.getProgressBarWidth()}">
                    </div>
                    ${this.getGraduationsHtml()}
                </div>`;
    }

    getGraduationsHtml() {
        let max = this.max();
        let graduations = '';
        for (let i = 0; i < max - 1; i++) {
            const left = (i + 1) * 100 / max;
            graduations += `<div class="graduation-bar" style="left:${left}%"></div>`;
        }
        return graduations;
    }

    getProgressWidth() {
        let max = this.max();
        let maxFullWidth = this.maxFullWidth();
        return `${(max / maxFullWidth) * 100}%`;
    }

    getProgressBarWidth() {
        let current = this.current();
        let max = this.max();
        let width = current / max * 100;
        if (width < 1)
            width = 1;
        return `${width}%`;
    }

    caption() {
        return this.dataset['caption'] ?? '';
    }

    tooltip() {
        return this.dataset['tooltip'] ?? '';
    }

    max() {
        return parseInt(this.dataset['max'] ?? '0', 10);
    }

    maxFullWidth() {
        return parseInt(this.dataset['maxFullWidth'] ?? '0', 10);
    }

    current() {
        return parseFloat(this.dataset['current'] ?? '0');
    }

    numberMedication() {
        return parseInt(this.dataset['numberMedication'] ?? '0', 10);
    }
}

customElements.define('time-progress-bar', TimeProgressBar);