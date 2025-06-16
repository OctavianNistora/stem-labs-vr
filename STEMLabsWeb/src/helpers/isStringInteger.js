export default function isStringInteger(string) {
    const number = Number(string);
    return Number.isInteger(number);
}
