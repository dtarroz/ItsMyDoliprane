import { ApiMedication, Medication } from "./api/_medication.js";

const $person = document.querySelector("#person") as HTMLSelectElement;
const $drug = document.querySelector("#drug") as HTMLSelectElement;
const $date = document.querySelector("#date") as HTMLInputElement;
const $hour = document.querySelector("#hour") as HTMLInputElement;
const $button = document.querySelector("#add") as HTMLButtonElement;

$person.addEventListener("change", () => {
    const personId = parseInt($person.value, 10);
    refresh(personId);
});

$button.addEventListener("click", () => {
    const medication = {
        personId: parseInt($person.value, 10),
        drugId: parseInt($drug.value, 10),
        date: $date.value,
        hour: $hour.value
    } as Medication;
    ApiMedication.add(medication).then(() => {
        refresh();
    })
    .catch((error: string) => {
        alert(error);
    });
});

// ---------------------------------------------------------

function refresh(personId?: number) {
    if (personId)
        document.location.href = `${document.location.origin}?personId=${personId}`;
    else
        document.location.reload();
}