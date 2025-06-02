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
import ViewRelatedReportsListPage from "./routes/ViewRelatedReportsListPage.tsx";
import ViewRelatedReportPage from "./routes/ViewRelatedReportPage.tsx";

function App() {
  return (
    <>
      <CssBaseline />
      <ThemeProvider theme={theme}>
        <BrowserRouter>
          <Routes>
            <Route element={<ToastLayout />}>
              <Route element={<AuthContextProvider />}>
                <Route element={<AuthenticationWrapper reverse />}>
                  <Route element={<AuthLayout />}>
                    <Route path="login" element={<LoginPage />} />
                    <Route path="recovery">
                      <Route index element={<RecoverySelectionPage />} />
                      <Route
                        path="username"
                        element={<RecoveryUsernamePage />}
                      />
                      <Route
                        path="password"
                        element={<RecoveryPasswordPage />}
                      />
                    </Route>
                  </Route>
                </Route>
                <Route element={<AuthenticationWrapper />}>
                  <Route element={<DrawerLayout />}>
                    <Route index element={<HomePage />} />
                    <Route path="reports">
                      <Route index element={<ViewRelatedLaboratoriesPage />} />
                      <Route path=":laboratoryId">
                        <Route index element={<ViewRelatedSessionsPage />} />
                        <Route path=":sessionId">
                          <Route
                            index
                            element={<ViewRelatedReportsListPage />}
                          />
                          <Route path=":userId/:reportId?">
                            <Route index element={<ViewRelatedReportPage />} />
                          </Route>
                        </Route>
                      </Route>
                    </Route>
                    <Route path="laboratories">
                      <Route index element={<ViewLaboratoriesPage />} />
                      <Route path=":laboratoryId">
                        <Route index element={<ManageLaboratoryPage />} />
                      </Route>
                    </Route>
                    <Route path="manage-users" element={<ManageUsersPage />} />
                    <Route path="add-user" element={<AddUserPage />} />
                    <Route path="profile" element={<ProfilePage />} />
                  </Route>
                </Route>
              </Route>
            </Route>
          </Routes>
        </BrowserRouter>
      </ThemeProvider>
    </>
  );
}

export default App;
