import { ApiMedication, Medication } from "./api/_medication.js";

const $person = document.querySelector("#person") as HTMLSelectElement;
const $drug = document.querySelector("#drug") as HTMLSelectElement;
const $customDateLink = document.querySelector("#custom-date-link") as HTMLAnchorElement;
const $customDate = document.querySelector("#custom-date") as HTMLDivElement;
const $date = document.querySelector("#date") as HTMLInputElement;
const $hour = document.querySelector("#hour") as HTMLInputElement;
const $button = document.querySelector("#add") as HTMLButtonElement;

$person.addEventListener("change", () => {
    const personId = parseInt($person.value, 10);
    refresh(personId);
});

$customDateLink.addEventListener('click', () => {
    $customDate.classList.add("active");
    const today = new Date();
    $date.value = today.toISOString().split('T')[0];
    $hour.value = today.toLocaleTimeString().substring(0, 5);
})

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