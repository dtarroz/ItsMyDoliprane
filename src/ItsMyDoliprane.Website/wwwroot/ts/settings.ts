import { ApiDrug } from './api/_drug.js';

const $rows = document.querySelectorAll<HTMLElement>('.settings-row');

$rows.forEach(row => {
    const drugId = parseInt(row.dataset['drugId'] ?? '', 10);
    const $checkboxes = row.querySelectorAll<HTMLInputElement>('input[type="checkbox"]');

    $checkboxes.forEach($checkbox => {
        $checkbox.addEventListener('change', () => {
            const forAdult = (row.querySelector<HTMLInputElement>('input[data-field="forAdult"]')?.checked) ?? false;
            const forChild = (row.querySelector<HTMLInputElement>('input[data-field="forChild"]')?.checked) ?? false;

            ApiDrug.update({ id: drugId, forAdult, forChild })
                .catch((_error: string) => {
                    $checkbox.checked = !$checkbox.checked;
                    alert("Erreur lors de la sauvegarde, veuillez réessayer.");
                });
        });
    });
});

