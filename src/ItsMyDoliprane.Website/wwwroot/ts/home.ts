import { ApiMedication, type NewMedication } from './api/_medication.js';
import './components/time-progress-bar.js';
import { ImlButton } from '../lib/itsmylib.components/iml-button.js';
import '../lib/itsmylib.components/iml-icon-pen-square.js';
import '../lib/itsmylib.components/iml-icon-trash.js';

const $persons = document.querySelectorAll<HTMLInputElement>('input[name="person"]');
const $currentPerson = () => document.querySelector('input[name="person"]:checked') as HTMLInputElement;
const $drug = document.querySelector('#drug') as HTMLSelectElement;
const $customDateLink = document.querySelector('#custom-date-link') as HTMLAnchorElement;
const $customDate = document.querySelector('#custom-date') as HTMLDivElement;
const $date = document.querySelector('#date') as HTMLInputElement;
const $hour = document.querySelector('#hour') as HTMLInputElement;
const $button = document.querySelector('#add') as ImlButton;
const $editMedication = document.querySelector('[data-medication-edit]') as HTMLElement | null;
const $deleteMedications = document.querySelectorAll<HTMLElement>('[data-medication-delete]');

document.addEventListener('visibilitychange', function () {
    if (document.visibilityState === 'visible')
        refresh();
});

$persons.forEach(e => e.addEventListener('change', () => {
    refresh(getCurrentPersonId());
}));

$customDateLink.addEventListener('click', () => {
    $customDate.classList.add('active');
    const today = new Date();
    $date.value = today.toISOString().split('T')[0];
    $hour.value = today.toLocaleTimeString().substring(0, 5);
});

$button.addEventListener('iml-button:click', () => {
    const medication = {
        personId: getCurrentPersonId(),
        drugId: parseInt($drug.value, 10),
        date: $date.value,
        hour: $hour.value
    } as NewMedication;
    ApiMedication.add(medication).then(() => {
        refresh();
    })
    .catch((error: string) => {
        alert(error);
    });
});

$editMedication?.addEventListener('click', () => {
    $deleteMedications.forEach(m => m.classList.toggle('icon-active'));
});

$deleteMedications.forEach(m => m.addEventListener('click', () => {
    if (confirm('Voulez-vous supprimer cette medication ?')) {
        const medicationId = parseInt(m.dataset['medicationId'] ?? '', 10);
        ApiMedication.delete(medicationId).then(() => {
            refresh();
        })
        .catch((error: string) => {
            alert(error);
        });
    }
}));

// ---------------------------------------------------------

function refresh(personId?: number) {
    if (personId)
        document.location.href = `${document.location.origin}${document.location.pathname}?personId=${personId}`;
    else
        document.location = document.location;
}

function getCurrentPersonId() {
    return parseInt($currentPerson().dataset['value'] ?? '', 10);
}