import type { ToastItem } from "../layouts/ToastLayout.tsx";
import { type Dispatch, type SetStateAction } from "react";
export declare function toastErrorMessageHandle(addToast: (toast: ToastItem) => void, setUser: Dispatch<SetStateAction<any>>, error: any): void;
