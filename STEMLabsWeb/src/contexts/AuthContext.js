import { jsx as _jsx } from "react/jsx-runtime";
import { createContext, useEffect, useMemo, useState, } from "react";
import axios from "axios";
import { Outlet } from "react-router";
export const AuthContext = createContext({
    user: null,
    setUser: () => null,
    isAuthInitialized: false,
});
export function AuthContextProvider() {
    const [user, setUser] = useState(null);
    const [isAuthInitialized, setIsAuthInitialized] = useState(false);
    useEffect(() => {
        const storageRefreshToken = localStorage.getItem("refreshToken");
        if (!storageRefreshToken) {
            setIsAuthInitialized(true);
            return;
        }
        const abortController = new AbortController();
        axios
            .post(`${import.meta.env.VITE_API_URL}/api/auth/refresh-token`, storageRefreshToken, {
            headers: {
                "Content-Type": "application/json",
            },
            signal: abortController.signal,
        })
            .then((res) => {
            const { uid, accessToken, refreshToken, role } = res.data;
            setUser({ uid, accessToken, role });
            localStorage.setItem("refreshToken", refreshToken);
            setIsAuthInitialized(true);
        })
            .catch((_) => {
            console.log("Refresh token expired");
            localStorage.removeItem("refreshToken");
            setIsAuthInitialized(true);
        });
        return () => abortController.abort();
    }, []);
    const authValue = useMemo(() => {
        return {
            user,
            setUser,
            isAuthInitialized,
        };
    }, [user, setUser, isAuthInitialized]);
    return (_jsx(AuthContext.Provider, { value: authValue, children: isAuthInitialized ? _jsx(Outlet, {}) : null }));
}
