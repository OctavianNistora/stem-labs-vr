import { jsx as _jsx, Fragment as _Fragment } from "react/jsx-runtime";
import { useContext, useEffect } from "react";
import { AuthContext } from "../../contexts/AuthContext.tsx";
import { Outlet, useNavigate } from "react-router";
import { CircularProgress } from "@mui/material";
export default function AuthenticationWrapper(props) {
    const { reverse } = props;
    const { user, isAuthInitialized } = useContext(AuthContext);
    const navigate = useNavigate();
    useEffect(() => {
        if (isAuthInitialized) {
            if (reverse && user) {
                navigate("/");
            }
            else if (!reverse && !user) {
                navigate("/login");
            }
        }
    }, [reverse, user, isAuthInitialized]);
    return _jsx(_Fragment, { children: isAuthInitialized ? _jsx(Outlet, {}) : _jsx(CircularProgress, {}) });
}
