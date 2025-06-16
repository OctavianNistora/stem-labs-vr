import { jsx as _jsx, jsxs as _jsxs, Fragment as _Fragment } from "react/jsx-runtime";
import { Box, List, ListItem, ListItemButton, ListItemText, } from "@mui/material";
import { Link, useNavigate, useParams } from "react-router";
import { useContext, useEffect, useState } from "react";
import { ToastContext } from "../layouts/ToastLayout.tsx";
import { AuthContext } from "../contexts/AuthContext.tsx";
import isStringPositiveInteger from "../helpers/isStringPositiveInteger.tsx";
import { toastErrorMessageHandle } from "../helpers/ToastErrorMessageHandle.tsx";
import TitleWithBackButton from "../components/TitleWithBackButton.tsx";
import Divider from "@mui/material/Divider";
import { axiosRequestWithAutoReauth } from "../helpers/axiosRequestWithAutoReauth.tsx";
export default function ViewRelatedParticipantsPage() {
    const { sessionId } = useParams();
    const [participants, setParticipants] = useState([]);
    const navigate = useNavigate();
    const { addToast } = useContext(ToastContext);
    const { user, setUser } = useContext(AuthContext);
    useEffect(() => {
        if (!isStringPositiveInteger(sessionId) || user?.role === "student") {
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
            const response = await axiosRequestWithAutoReauth({
                method: "GET",
                url: `${import.meta.env.VITE_API_URL}/api/laboratory-sessions/${sessionId}/participants`,
                headers: {
                    Authorization: `Bearer ${user.accessToken}`,
                },
            }, setUser);
            setParticipants(response.data.map((participant) => ({
                id: participant.id,
                name: participant.name,
                date: new Date(participant.date),
            })));
        }
        catch (error) {
            toastErrorMessageHandle(addToast, setUser, error);
        }
    }
    return (_jsxs(Box, { display: "flex", flexDirection: "column", width: "800px", height: "800px", bgcolor: "white", boxShadow: 4, overflow: "visible", alignItems: "center", children: [_jsx(TitleWithBackButton, { to: "..", title: "Select a participant" }), _jsx(Box, { width: "100%", overflow: "auto", children: _jsx(List, { id: "participants-list", sx: { width: "100%" }, children: participants.map((session) => {
                        const sessionDate = session.date;
                        const date = sessionDate.getDate().toString().padStart(2, "0") +
                            "/" +
                            (sessionDate.getMonth() + 1).toString().padStart(2, "0") +
                            "/" +
                            sessionDate.getFullYear();
                        const time = sessionDate.getHours().toString().padStart(2, "0") +
                            ":" +
                            sessionDate.getMinutes().toString().padStart(2, "0") +
                            ":" +
                            sessionDate.getSeconds().toString().padStart(2, "0") +
                            " UTC" +
                            (sessionDate.getTimezoneOffset() < 0 ? "+" : "-") +
                            sessionDate.getTimezoneOffset() / -60;
                        const text = date + ", " + time;
                        return (_jsxs(_Fragment, { children: [_jsx(Divider, {}), _jsx(ListItem, { id: session.id.toString(), disablePadding: true, children: _jsxs(ListItemButton, { component: Link, to: session.id +
                                            (user?.role == "student" ? `/${user.uid}` : ""), children: [_jsx(ListItemText, { primary: session.name }), _jsx(ListItemText, { primary: _jsx(Box, { textAlign: "right", children: text }) })] }) }, session.id)] }));
                    }) }) })] }));
}
