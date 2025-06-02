export default function isStringPositiveInteger(
  string: string | undefined,
): boolean {
  const number = Number(string);
  return Number.isInteger(number) && number >= 0;
}
