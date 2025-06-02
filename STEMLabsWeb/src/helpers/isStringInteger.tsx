export default function isStringInteger(string: string | undefined): boolean {
  const number = Number(string);
  return Number.isInteger(number);
}
