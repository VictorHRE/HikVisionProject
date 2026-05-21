import type {
  Employee,
  ContiendaStore,
  /* HistoryEntry, */ LoginRequest,
  LoginResponse,
} from "./types";
import type { CatalogOption } from "./types";

const BASE = (import.meta.env.VITE_API_URL as string) || "";
console.log(BASE);
export const API_ENABLED = Boolean(BASE);

type StorageSchema = {
  employees: Employee[];
  branches: ContiendaStore[] /*, history: HistoryEntry[] */;
};
const LS_KEY = "fp_storage_v1";

function loadLocal(): StorageSchema {
  const raw = localStorage.getItem(LS_KEY);
  if (!raw) {
    const initial: StorageSchema = {
      employees: [],
      branches: [] /*, history: [] */,
    };
    localStorage.setItem(LS_KEY, JSON.stringify(initial));
    return initial;
  }
  try {
    return JSON.parse(raw) as StorageSchema;
  } catch {
    const initial: StorageSchema = {
      employees: [],
      branches: [] /*, history: [] */,
    };
    localStorage.setItem(LS_KEY, JSON.stringify(initial));
    return initial;
  }
}

function saveLocal(s: StorageSchema) {
  localStorage.setItem(LS_KEY, JSON.stringify(s));
}

export async function getEmployees(): Promise<Employee[]> {
  if (API_ENABLED) {
    try {
      const data = await request<{ value: Array<Record<string, unknown>> }>(
        "/employee/get-employees"
      );
      const users = data?.value ?? [];
      // Map external user to local Employee type
      const toStr = (v: unknown) => (typeof v === "string" ? v : "");
      return users.map((uObj) => {
        const u = uObj as Record<string, unknown>;
        const first = toStr(u["name"]);
        const last = toStr(u["lastName"]);
        return {
          id: (u["id"] ?? "") as number | string,
          name:
            first ||
            toStr(u["employeeInternalId"]) ||
            toStr(u["email"]) ||
            last,
          active: u["status"] === "ACTIVE",
          finger:
            typeof u["finger"] === "string"
              ? u["finger"]
              : Array.isArray(u["fingerprints"]),
          birthDate:
            typeof u["birthdate"] === "string"
              ? u["birthdate"]
              : typeof u["birthDate"] === "string"
              ? u["birthDate"]
              : undefined,
          email: typeof u["email"] === "string" ? u["email"] : undefined,
          lastName:
            typeof u["lastName"] === "string"
              ? u["lastName"]
              : typeof u["lastname"] === "string"
              ? u["lastname"]
              : undefined,
          status: typeof u["status"] === "string" ? u["status"] : undefined,
          identification:
            typeof u["identification"] === "string"
              ? u["identification"]
              : undefined,
          position:
            typeof u["position"] === "string" ? u["position"] : undefined,
          phone: typeof u["phone"] === "string" ? u["phone"] : undefined,
          idStoreHQ:
            typeof u["idStoreHQ"] === "number"
              ? u["idStoreHQ"]
              : typeof u["idStoreHQ"] === "string"
              ? Number(u["idStoreHQ"])
              : undefined,
          userType: typeof u["userType"] === "string" ? u["userType"] : null,
          gender: typeof u["gender"] === "string" ? u["gender"] : null,
          beginTime:
            typeof u["beginTime"] === "string" ? u["beginTime"] : undefined,
          endTime: typeof u["endTime"] === "string" ? u["endTime"] : undefined,
          createdAt:
            typeof u["createdAt"] === "string" ? u["createdAt"] : undefined,
        } as Employee;
      });
    } catch (e) {
      console.log(e)
      alert("Su sesión ha expirado. Redirigiendo a login.");
      setTimeout(() => {
        try {
          window.location.replace("/login");
        } catch {
          /* ignorar */
        }
      }, 0);
      console.warn(
        "API getEmployees (employee/get-employees) failed, using local",
        e
      );
    }
  }
  return [];
}

// Obtener un empleado por ID
export async function getEmployeeById(
  id: string | number
): Promise<Employee | null> {
  if (API_ENABLED) {
    try {
      const data = await request<
        { value?: Record<string, unknown> } | Record<string, unknown>
      >("/employee/get-employee-by-id", {
        method: "POST",
        body: JSON.stringify(String(id)),
      });
      const obj = (
        data && "value" in data
          ? (data as { value?: Record<string, unknown> }).value
          : data
      ) as Record<string, unknown> | undefined;
      if (!obj) return null;
      const toStr = (v: unknown) => (typeof v === "string" ? v : "");
      const first = toStr(obj["name"]);
      const last = toStr(obj["lastName"]);
      return {
        id: (obj["id"] ?? "") as number | string,
        name:
          first ||
          toStr(obj["employeeInternalId"]) ||
          toStr(obj["email"]) ||
          last,
        active: obj["status"] === "ACTIVE",
        finger:
          typeof obj["finger"] === "string"
            ? obj["finger"]
            : Array.isArray(obj["fingerprints"]),
        birthDate:
          typeof obj["birthdate"] === "string"
            ? obj["birthdate"]
            : typeof obj["birthDate"] === "string"
            ? obj["birthDate"]
            : undefined,
        email: typeof obj["email"] === "string" ? obj["email"] : undefined,
        lastName:
          typeof obj["lastName"] === "string"
            ? obj["lastName"]
            : typeof obj["lastname"] === "string"
            ? obj["lastname"]
            : undefined,
        status: typeof obj["status"] === "string" ? obj["status"] : undefined,
        identification:
          typeof obj["identification"] === "string"
            ? obj["identification"]
            : undefined,
        position:
          typeof obj["position"] === "string" ? obj["position"] : undefined,
        phone: typeof obj["phone"] === "string" ? obj["phone"] : undefined,
        idStoreHQ:
          typeof obj["idStoreHQ"] === "number"
            ? obj["idStoreHQ"]
            : typeof obj["idStoreHQ"] === "string"
            ? Number(obj["idStoreHQ"])
            : undefined,
        userType: typeof obj["userType"] === "string" ? obj["userType"] : null,
        gender: typeof obj["gender"] === "string" ? obj["gender"] : null,
        beginTime:
          typeof obj["beginTime"] === "string" ? obj["beginTime"] : undefined,
        endTime:
          typeof obj["endTime"] === "string" ? obj["endTime"] : undefined,
        createdAt:
          typeof obj["createdAt"] === "string" ? obj["createdAt"] : undefined,
      } as Employee;
    } catch (e) {
      alert("Su sesión ha expirado. Redirigiendo a login.");
      setTimeout(() => {
        try {
          window.location.replace("/login");
        } catch {
          /* ignorar */
        }
      }, 0);
      console.warn(
        "API getEmployeeById (employee/get-employee-by-id) failed",
        e
      );
    }
  }
  return null;
}

export async function getContiendaStores(): Promise<ContiendaStore[]> {
  if (API_ENABLED) {
    try {
      const data = await request<{ value?: Array<Record<string, unknown>> }>(
        "/contienda/getcontienda"
      );
      const arr = data?.value ?? [];
      return arr.map((item) => {
        const r = item as Record<string, unknown>;
        return {
          id: typeof r["id"] === "number" ? r["id"] : Number(r["id"]) || 0,
          idStoreHQ:
            typeof r["idStoreHQ"] === "number"
              ? r["idStoreHQ"]
              : typeof r["idStoreHQ"] === "string"
              ? Number(r["idStoreHQ"])
              : undefined,
          storeName:
            typeof r["storeName"] === "string"
              ? r["storeName"]
              : String(r["storeName"] ?? ""),
        } as ContiendaStore;
      });
    } catch (e) {
      alert("Su sesión ha expirado. Redirigiendo a login.");
      setTimeout(() => {
        try {
          window.location.replace("/login");
        } catch {
          /* ignorar */
        }
      }, 0);
      console.warn("API getContiendaStores failed", e);
    }
  }
  return [];
}

export async function addEmployeeToDevice(emp: Employee) {
  if (API_ENABLED) {
    const resp = await request<Record<string, unknown>>(
      "/employee/add-employeetodevice",
      { method: "POST", body: JSON.stringify(emp) }
    );
    const typed = resp as unknown as {
      status?: number;
      message?: string;
      isSuccess?: boolean;
    };
    if (typeof typed?.status === "number") {
      if (typed.status === 0) return resp;
      throw new Error(String(typed.message ?? `status ${typed.status}`));
    }
    if (typeof typed?.isSuccess === "boolean") {
      if (typed.isSuccess) return resp;
      throw new Error(String(typed.message ?? "Request not successful"));
    }
    return resp;
  }
  const s = loadLocal();
  const e = s.employees.find((x) => x.id === emp.id);
  if (!e) {
    s.employees.push(emp);
    saveLocal(s);
    return emp;
  }
  (e as Record<string, unknown>).idStoreHQ = emp.idStoreHQ;
  // update other fields if present
  e.name = emp.name ?? e.name;
  e.lastName = emp.lastName ?? e.lastName;
  e.email = emp.email ?? e.email;
  e.phone = emp.phone ?? e.phone;
  e.position = emp.position ?? e.position;
  e.status = emp.status ?? e.status;
  saveLocal(s);
  return e;
}

export async function updateEmployeeToDevice(emp: Employee) {
  if (API_ENABLED) {
    const resp = await request<Record<string, unknown>>(
      "/employee/update-employeetodevice",
      { method: "POST", body: JSON.stringify(emp) }
    );
    const typed = resp as unknown as {
      status?: number;
      message?: string;
      isSuccess?: boolean;
    };
    if (typeof typed?.status === "number") {
      if (typed.status === 0) return resp;
      throw new Error(String(typed.message ?? `status ${typed.status}`));
    }
    if (typeof typed?.isSuccess === "boolean") {
      if (typed.isSuccess) return resp;
      throw new Error(String(typed.message ?? "Request not successful"));
    }
    return resp;
  }

  const s = loadLocal();
  const e = s.employees.find((x) => x.id === emp.id);
  if (!e) throw new Error("employee not found");
  (e as Record<string, unknown>).idStoreHQ = emp.idStoreHQ;
  e.name = emp.name ?? e.name;
  e.lastName = emp.lastName ?? e.lastName;
  e.email = emp.email ?? e.email;
  e.phone = emp.phone ?? e.phone;
  e.position = emp.position ?? e.position;
  e.status = emp.status ?? e.status;
  saveLocal(s);
  return e;
}

export async function getCatalog(
  type: "STATUS" | "TYPE" | "GENDER"
): Promise<CatalogOption[]> {
  if (API_ENABLED) {
    try {
      const resp = await request<{ value?: Array<Record<string, unknown>> }>(
        "/catalogo/getcatalogo",
        { method: "POST", body: JSON.stringify(type) }
      );
      const arr = resp?.value ?? [];
      const opts = arr.map((x: Record<string, unknown>) => {
        const descripcion = String(
          x["descripcion"] ?? x["label"] ?? x["value"] ?? ""
        );
        const codigo = String(x["codigo"] ?? x["id"] ?? descripcion);
        return { value: codigo, label: descripcion };
      });
      return opts;
    } catch (e) {
      console.warn("API getCatalog failed", e);
    }
  }

  return [];
}

export async function addFingerprintedEmployee(emp: Employee) {
  // Returns an object { success: boolean, message?: string, data?: unknown }
  if (API_ENABLED) {
    try {
      const resp = await request<Record<string, unknown>>(
        "/employee/add-fingerprintedemployee",
        { method: "POST", body: JSON.stringify(emp) }
      );
      const typed = resp as unknown as {
        status?: number;
        message?: string;
        isSuccess?: boolean;
      };
      if (typeof typed?.status === "number") {
        if (typed.status === 0) return { success: true, data: resp };
        return {
          success: false,
          message: String(typed.message ?? `status ${typed.status}`),
          data: resp,
        };
      }
      if (typeof typed?.isSuccess === "boolean") {
        if (typed.isSuccess) return { success: true, data: resp };
        return {
          success: false,
          message: String(typed.message ?? "Request not successful"),
          data: resp,
        };
      }
      return { success: true, data: resp };
    } catch (e) {
      return {
        success: false,
        message: String(e instanceof Error ? e.message : e),
      };
    }
  }
  const s = loadLocal();
  const e = s.employees.find((x) => x.id === emp.id);
  if (!e) return { success: false, message: "employee not found" };
  // merge fields from payload
  e.name = emp.name ?? e.name;
  e.lastName = emp.lastName ?? e.lastName;
  e.idStoreHQ = emp.idStoreHQ ?? e.idStoreHQ;
  e.status = emp.status ?? e.status;
  saveLocal(s);
  return { success: true, data: e };
}

export async function deleteFingerprintedEmployee(emp: Employee) {
  if (API_ENABLED) {
    const resp = await request<Record<string, unknown>>(
      "/employee/delete-fingerprintedemployee",
      { method: "DELETE", body: JSON.stringify(emp) }
    );
    const typed = resp as unknown as {
      status?: number;
      message?: string;
      isSuccess?: boolean;
    };
    if (typeof typed?.status === "number") {
      if (typed.status === 0) return resp;
      throw new Error(String(typed.message ?? `status ${typed.status}`));
    }
    if (typeof typed?.isSuccess === "boolean") {
      if (typed.isSuccess) return resp;
      throw new Error(String(typed.message ?? "Request not successful"));
    }
    return resp;
  }

  // Local fallback: remove fingerprints from local copy
  const s = loadLocal();
  const e = s.employees.find((x) => x.id === emp.id);
  if (!e) throw new Error("employee not found");
  saveLocal(s);
  return e;
}

/* export async function getHistory(filters?: { employeeId?: string; branchId?: string; from?: string; to?: string; type?: 'in' | 'out' }) {
	if (API_ENABLED) {
		try {
			const p = new URLSearchParams()
			if (filters) Object.entries(filters).forEach(([k, v]) => { if (v) p.set(k, String(v)) })
			return await request<HistoryEntry[]>(`/history${p.toString() ? `?${p.toString()}` : ''}`)
		} catch (e) { console.warn('API getHistory failed, using local', e) }
	}
	// No usar historial local por defecto (comentando loadLocal)
	// const s = loadLocal()
	// return s.history.filter(h => { [filter logic commented out] })
	// Historial deshabilitado: retornar arreglo vacío. Para reactivar, restaurar el bloque original.
	return []
} */

/* export async function addHistory(entry: HistoryEntry) {
		if (API_ENABLED) {
			try { return await request<HistoryEntry>('/history', { method: 'POST', body: JSON.stringify(entry) }) } catch (e) { console.warn('API addHistory failed, using local', e) }
	}
	// addHistory deshabilitado — no guardar historial local
	// const s = loadLocal()
	// s.history.push(entry)
	// saveLocal(s)
	// return entry
	return entry
} */

// --- Authentication / login
export async function loginWithUserNameOrEmail(
  name: string,
  password: string
): Promise<{
  success: boolean;
  message?: string;
  token?: string;
  user?: unknown;
}> {
  if (!API_ENABLED) return { success: false, message: "API no configurada" };

  const payload: LoginRequest = { id: 0, name, email: "string", password };

  // Allow custom path via VITE_LOGIN_PATH; otherwise fallback based on BASE
  const loginPath =
    (import.meta.env.VITE_LOGIN_PATH as string) ||
    (BASE.endsWith("/api")
      ? "/authentication/login"
      : "/api/authentication/login");

  try {
    const res = await request<LoginResponse>(loginPath, {
      method: "POST",
      body: JSON.stringify(payload),
    });
    // The backend returns { isSuccess: true, message: 'OK' } — map isSuccess -> success
    return {
      success: Boolean(res?.isSuccess),
      message: res?.message,
      token: res?.token,
      user: res?.user,
    };
  } catch (e) {
    console.error("loginWithUserNameOrEmail failed", e);
    return {
      success: false,
      message: String(e instanceof Error ? e.message : e),
    };
  }
}

export async function getToken(
  name: string,
  password: string
): Promise<{
  isSuccess: boolean;
  token?: string;
  message?: string;
  user?: unknown;
}> {
  if (!API_ENABLED) return { isSuccess: false, message: "API no configurada" };
  try {
    const payload = { id: 0, name, email: "string", password };
    // Ajuste: evitar duplicar /api si BASE ya termina en /api
    const tokenPath =
      (import.meta.env.VITE_GETTOKEN_PATH as string) ||
      (BASE.endsWith("/api")
        ? "/authentication/gettoken"
        : "/api/authentication/gettoken");
    const res = await request<{
      isSuccess?: boolean;
      token?: string;
      message?: string;
      user?: unknown;
    }>(tokenPath, { method: "POST", body: JSON.stringify(payload) });
    return {
      isSuccess: Boolean(res?.isSuccess),
      token: res?.token,
      message: res?.message,
      user: res?.user,
    };
  } catch (e) {
    console.error("getToken failed", e);
    return {
      isSuccess: false,
      message: String(e instanceof Error ? e.message : e),
    };
  }
}

// --- HTTP helper
async function request<T = unknown>(
  path: string,
  opts: RequestInit = {}
): Promise<T> {
  if (!API_ENABLED) throw new Error("API not configured (VITE_API_URL)");

  const headers = new Headers(opts.headers as HeadersInit);

  const token = localStorage.getItem("fp_token");
  if (token) {
    // If the token looks like a JWT add Authorization header (best-effort)
    headers.set("Authorization", `Bearer ${token}`);
  }

  // Only set Content-Type when we actually have a body and it's not a FormData.
  // Some GET endpoints reject requests with a content-type header when there's no body, so avoid setting it for GET/no-body.
  if (opts.body !== undefined && !(opts.body instanceof FormData)) {
    headers.set("Content-Type", "application/json");
  }

  const url = path.startsWith("/") ? `${BASE}${path}` : `${BASE}/${path}`;
  const res = await fetch(url, { ...opts, headers });
  if (!res.ok) {
    //const txt = await res.text().catch(() => '')
    const response = await res.json();

    throw new Error(
      `${res.status} ${res.statusText} - ${
        response.message ?? response.value?.message ?? ""
      } `
    );
  }
  if (res.status === 204) return undefined as unknown as T;
  return await res.json();
}
