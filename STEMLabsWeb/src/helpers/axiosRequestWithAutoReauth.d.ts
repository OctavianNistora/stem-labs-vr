import { type AxiosRequestConfig, type AxiosResponse } from "axios";
import type { Dispatch, SetStateAction } from "react";
import type { User } from "../contexts/AuthContext.tsx";
export declare function axiosRequestWithAutoReauth<T = any, R = AxiosResponse<T>, D = any>(config: AxiosRequestConfig<D>, setUser: Dispatch<SetStateAction<User | null>>): Promise<R>;
