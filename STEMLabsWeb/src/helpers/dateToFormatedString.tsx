export default function dateToFormattedString(date: Date): string {
  console.log(date);
  const dateString =
    date.getDate().toString().padStart(2, "0") +
    "/" +
    (date.getMonth() + 1).toString().padStart(2, "0") +
    "/" +
    date.getFullYear();

  const timeString =
    date.getHours().toString().padStart(2, "0") +
    ":" +
    date.getMinutes().toString().padStart(2, "0") +
    ":" +
    date.getSeconds().toString().padStart(2, "0") +
    " UTC" +
    (date.getTimezoneOffset() < 0 ? "+" : "-") +
    date.getTimezoneOffset() / -60;

  return dateString + ", " + timeString;
}
