# SEC-103 — Evidence index

All screenshots below are included under `screenshots/`. Hashes are SHA-256 of these files.

| File | Session | What it shows | SHA-256 |
|------|---------|---------------|---------|
| [`screenshots/01-wazuh-dashboard-first-login.png`](screenshots/01-wazuh-dashboard-first-login.png) | 1 | Wazuh Overview after first dashboard access at `https://192.168.169.140`; no agents registered yet; self-signed TLS “Not secure” warning visible | `a5663642b5003bda60d407cc7c52325d8cae7d57e0a57db06590c1786c0c00a9` |
| [`screenshots/02-agent-active-vm-window11.png`](screenshots/02-agent-active-vm-window11.png) | 2 | Agent **VM-Window11 (001)** Active; IP `192.168.169.11`; Wazuh v4.14.10; Windows 11 inventory | `fa445a591ef56ed0fc615056c362147371d5c0cccaf782be0102899d68825915` |
| [`screenshots/03-discover-seven-4625-events.png`](screenshots/03-discover-seven-4625-events.png) | 3 | Discover: `agent.id: "001" AND data.win.system.eventID: "4625"` → **7 hits**; agent VM-Window11 | `acff09275487fe1c174666ad5a7f3a4b49389b56dfe08a16016eb4f4c5fc6b02` |
| [`screenshots/04-fakeuser-event-details.png`](screenshots/04-fakeuser-event-details.png) | 3 | Expanded fields: `targetUserName` = fakeuser; `subjectUserName` = Administrator | `cc83fd04e98419d4e7149f2af3473f6d3ee34d9c366ad3a289ecd1f8989b187d` |
| [`screenshots/05-failed-logon-4625-full.png`](screenshots/05-failed-logon-4625-full.png) | 3 | Full 4625 alert: rule “Logon Failure - Unknown user or bad password”; logon type 2; source `::1` | `b5aaf2ebc8d7357258ae9fad69675b7097a93b0c3a33a2f14a6a8298627a28cd` |

## Intentionally excluded

- Password / login form screenshots
- Installer credential banners or `wazuh-install-files.tar` archives
- Plaintext credentials in any markdown
- Unrelated Group Policy / 1129 screenshots used only while navigating Discover
- Prior draft Session-1 how-to files that incorrectly fixed Ubuntu 24.04 or claimed official sizing support

## Missing evidence

The five screenshots listed above are present in this folder. No screenshot content was fabricated.
