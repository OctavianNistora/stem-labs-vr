import type { ToastItem } from "../layouts/ToastLayout.tsx";
import { type Dispatch, type SetStateAction } from "react";

export function toastErrorMessageHandle(
  addToast: (toast: ToastItem) => void,
  setUser: Dispatch<SetStateAction<any>>,
  error: any,
): void {
  if (error.response) {
    if (error.response.status === 500) {
      addToast({
        message: "The server encountered an error. Please try again later.",
        variant: "error",
      });
    } else {
      addToast({ message: error.response?.data, variant: "error" });
    }
  } else if (error.request) {
    setUser(null);
    localStorage.removeItem("refreshToken");

    addToast({
      message:
        "No response from the server. Please check your connection or try again later.",
      variant: "error",
    });
  } else {
    addToast({
      message:
        "An unexpected error occurred. Contact support if the issue persists.",
      variant: "error",
    });
  }
}
