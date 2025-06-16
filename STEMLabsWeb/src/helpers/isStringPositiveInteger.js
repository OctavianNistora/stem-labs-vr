export default function isStringPositiveInteger(string) {
    const number = Number(string);
    return Number.isInteger(number) && number >= 0;
}
