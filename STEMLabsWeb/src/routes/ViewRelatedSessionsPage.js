import { jsx as _jsx, Fragment as _Fragment, jsxs as _jsxs } from "react/jsx-runtime";
import { Box, List, ListItem, ListItemButton, ListItemText, } from "@mui/material";
import { Link, useNavigate, useParams } from "react-router";
import Divider from "@mui/material/Divider";
import { useContext, useEffect, useState } from "react";
import { ToastContext } from "../layouts/ToastLayout.tsx";
import { AuthContext } from "../contexts/AuthContext.tsx";
import { toastErrorMessageHandle } from "../helpers/ToastErrorMessageHandle.tsx";
import isStringPositiveInteger from "../helpers/isStringPositiveInteger.tsx";
import TitleWithBackButton from "../components/TitleWithBackButton.tsx";
import dateToFormattedString from "../helpers/dateToFormatedString.tsx";
import { axiosRequestWithAutoReauth } from "../helpers/axiosRequestWithAutoReauth.tsx";
export default function ViewRelatedSessionsPage() {
    const { laboratoryId } = useParams();
    const [sessions, setSessions] = useState([]);
    const navigate = useNavigate();
    const { addToast } = useContext(ToastContext);
    const { user, setUser } = useContext(AuthContext);
    useEffect(() => {
        if (!isStringPositiveInteger(laboratoryId)) {
            navigate("..");
            return;
        }
        fetchSessions();
    }, []);
    async function fetchSessions() {
        if (!user) {
            return;
        }
        try {
            let response;
            if (user.role.toLowerCase() === "admin") {
                response = await axiosRequestWithAutoReauth({
                    method: "GET",
                    url: `${import.meta.env.VITE_API_URL}/api/laboratories/${laboratoryId}/sessions`,
                    headers: {
                        Authorization: `Bearer ${user.accessToken}`,
                    },
                }, setUser);
            }
            else {
                response = await axiosRequestWithAutoReauth({
                    method: "GET",
                    url: `${import.meta.env.VITE_API_URL}/api/users/${user.uid}/related-laboratories/${laboratoryId}/sessions`,
                    headers: {
                        Authorization: `Bearer ${user.accessToken}`,
                    },
                }, setUser);
            }
            setSessions(response.data.map((session) => ({
                id: session.id,
                date: new Date(session.date),
            })));
        }
        catch (error) {
            toastErrorMessageHandle(addToast, setUser, error);
        }
    }
    return (_jsxs(Box, { display: "flex", flexDirection: "column", width: "600px", height: "800px", bgcolor: "white", boxShadow: 4, overflow: "visible", alignItems: "center", children: [_jsx(TitleWithBackButton, { to: "..", title: "Select a session's date and time" }), _jsx(Box, { width: "100%", overflow: "auto", children: _jsx(List, { id: "sessions-list", sx: { width: "100%" }, children: sessions.map((session) => {
                        return (_jsxs(_Fragment, { children: [_jsx(Divider, {}), _jsx(ListItem, { id: session.id.toString(), disablePadding: true, children: _jsx(ListItemButton, { component: Link, to: session.id +
                                            (user?.role.toLowerCase() == "student"
                                                ? `/${user.uid}`
                                                : ""), children: _jsx(ListItemText, { primary: dateToFormattedString(session.date) }) }) }, session.id)] }));
                    }) }) })] }));
}
