import { jsx as _jsx, jsxs as _jsxs, Fragment as _Fragment } from "react/jsx-runtime";
import { BrowserRouter, Route, Routes } from "react-router";
import LoginPage from "./routes/LoginPage.tsx";
import "./root.css";
import { AuthContextProvider } from "./contexts/AuthContext.tsx";
import AuthLayout from "./layouts/AuthLayout.tsx";
import AuthenticationWrapper from "./components/wrappers/AuthenticationWrapper.tsx";
import RecoverySelectionPage from "./routes/RecoverySelectionPage.tsx";
import RecoveryUsernamePage from "./routes/RecoveryUsernamePage.tsx";
import ToastLayout from "./layouts/ToastLayout.tsx";
import RecoveryPasswordPage from "./routes/RecoveryPasswordPage.tsx";
import DrawerLayout from "./layouts/DrawerLayout.tsx";
import { CssBaseline, ThemeProvider } from "@mui/material";
import { theme } from "./theme.tsx";
import HomePage from "./routes/HomePage.tsx";
import ProfilePage from "./routes/ProfilePage.tsx";
import ManageUsersPage from "./routes/ManageUsersPage.tsx";
import AddUserPage from "./routes/AddUserPage.tsx";
import ViewLaboratoriesPage from "./routes/ViewLaboratoriesPage.tsx";
import ManageLaboratoryPage from "./routes/ManageLaboratoryPage.tsx";
import ViewRelatedLaboratoriesPage from "./routes/ViewRelatedLaboratoriesPage.tsx";
import ViewRelatedSessionsPage from "./routes/ViewRelatedSessionsPage.tsx";
import ViewRelatedParticipantsPage from "./routes/ViewRelatedParticipantsPage.tsx";
import ViewRelatedReportPage from "./routes/ViewRelatedReportPage.tsx";
function App() {
    return (_jsxs(_Fragment, { children: [_jsx(CssBaseline, {}), _jsx(ThemeProvider, { theme: theme, children: _jsx(BrowserRouter, { children: _jsx(Routes, { children: _jsx(Route, { element: _jsx(ToastLayout, {}), children: _jsxs(Route, { element: _jsx(AuthContextProvider, {}), children: [_jsx(Route, { element: _jsx(AuthenticationWrapper, { reverse: true }), children: _jsxs(Route, { element: _jsx(AuthLayout, {}), children: [_jsx(Route, { path: "login", element: _jsx(LoginPage, {}) }), _jsxs(Route, { path: "recovery", children: [_jsx(Route, { index: true, element: _jsx(RecoverySelectionPage, {}) }), _jsx(Route, { path: "username", element: _jsx(RecoveryUsernamePage, {}) }), _jsx(Route, { path: "password", element: _jsx(RecoveryPasswordPage, {}) })] })] }) }), _jsx(Route, { element: _jsx(AuthenticationWrapper, {}), children: _jsxs(Route, { element: _jsx(DrawerLayout, {}), children: [_jsx(Route, { index: true, element: _jsx(HomePage, {}) }), _jsxs(Route, { path: "reports", children: [_jsx(Route, { index: true, element: _jsx(ViewRelatedLaboratoriesPage, {}) }), _jsxs(Route, { path: ":laboratoryId", children: [_jsx(Route, { index: true, element: _jsx(ViewRelatedSessionsPage, {}) }), _jsxs(Route, { path: ":sessionId", children: [_jsx(Route, { index: true, element: _jsx(ViewRelatedParticipantsPage, {}) }), _jsx(Route, { path: ":userId/:reportId?", children: _jsx(Route, { index: true, element: _jsx(ViewRelatedReportPage, {}) }) })] })] })] }), _jsxs(Route, { path: "laboratories", children: [_jsx(Route, { index: true, element: _jsx(ViewLaboratoriesPage, {}) }), _jsx(Route, { path: ":laboratoryId", children: _jsx(Route, { index: true, element: _jsx(ManageLaboratoryPage, {}) }) })] }), _jsx(Route, { path: "manage-users", element: _jsx(ManageUsersPage, {}) }), _jsx(Route, { path: "add-user", element: _jsx(AddUserPage, {}) }), _jsx(Route, { path: "profile", element: _jsx(ProfilePage, {}) })] }) })] }) }) }) }) })] }));
}
export default App;
