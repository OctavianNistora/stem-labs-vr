import { type Dispatch, type SetStateAction } from "react";
export type User = {
    uid: string;
    accessToken: string;
    role: "student" | "professor" | "admin";
};
type AuthState = {
    user: User | null;
    setUser: Dispatch<SetStateAction<User | null>>;
    isAuthInitialized: boolean;
};
export declare const AuthContext: import("react").Context<AuthState>;
export declare function AuthContextProvider(): import("react/jsx-runtime").JSX.Element;
export {};
