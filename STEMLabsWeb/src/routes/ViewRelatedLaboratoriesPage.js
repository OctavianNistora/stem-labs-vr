import { jsx as _jsx, Fragment as _Fragment, jsxs as _jsxs } from "react/jsx-runtime";
import { Box, List, ListItem, ListItemButton, ListItemText, Typography, } from "@mui/material";
import { useContext, useEffect, useState } from "react";
import { ToastContext } from "../layouts/ToastLayout.tsx";
import { AuthContext } from "../contexts/AuthContext.tsx";
import { toastErrorMessageHandle } from "../helpers/ToastErrorMessageHandle.tsx";
import Divider from "@mui/material/Divider";
import { Link } from "react-router";
import { axiosRequestWithAutoReauth } from "../helpers/axiosRequestWithAutoReauth.tsx";
export default function ViewRelatedLaboratoriesPage() {
    const [laboratories, setLaboratories] = useState([]);
    const { addToast } = useContext(ToastContext);
    const { user, setUser } = useContext(AuthContext);
    useEffect(() => {
        fetchLaboratories();
    }, []);
    async function fetchLaboratories() {
        if (!user) {
            return;
        }
        try {
            let response;
            if (user.role.toLowerCase() === "admin") {
                response = await axiosRequestWithAutoReauth({
                    method: "GET",
                    url: `${import.meta.env.VITE_API_URL}/api/laboratories/simplified`,
                    headers: {
                        Authorization: `Bearer ${user.accessToken}`,
                    },
                }, setUser);
            }
            else {
                response = await axiosRequestWithAutoReauth({
                    method: "GET",
                    url: `${import.meta.env.VITE_API_URL}/api/users/${user.uid}/related-laboratories`,
                    headers: {
                        Authorization: `Bearer ${user.accessToken}`,
                    },
                }, setUser);
            }
            setLaboratories(response.data);
        }
        catch (error) {
            toastErrorMessageHandle(addToast, setUser, error);
        }
    }
    return (_jsxs(Box, { display: "flex", flexDirection: "column", width: "600px", height: "800px", bgcolor: "white", boxShadow: 4, overflow: "auto", alignItems: "center", children: [_jsx(Typography, { variant: "h4", marginY: 2, children: "Select a laboratory" }), _jsx(List, { id: "laboratories-list", sx: { width: "100%" }, children: laboratories.map((laboratory) => (_jsxs(_Fragment, { children: [_jsx(Divider, {}), _jsx(ListItem, { id: laboratory.id.toString(), disablePadding: true, children: _jsx(ListItemButton, { component: Link, to: laboratory.id.toString(), children: _jsx(ListItemText, { primary: laboratory.name }) }) }, laboratory.id)] }))) })] }));
}
