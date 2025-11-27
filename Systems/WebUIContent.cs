using System.Text;

namespace cvbhhnClassLibrary1.Systems
{
    public static class WebUIContent
    {
        public static string GetHTML()
        {
            StringBuilder html = new StringBuilder();
            
            html.Append("<!DOCTYPE html><html lang=\"zh-CN\"><head>");
            html.Append("<meta charset=\"UTF-8\">");
            html.Append("<meta name=\"viewport\" content=\"width=device-width, initial-scale=1.0\">");
            html.Append("<title>OmniGauge - 数据分析工具</title>");
            // 引入 Remix Icon
            html.Append("<link href=\"https://cdn.jsdelivr.net/npm/remixicon@3.5.0/fonts/remixicon.css\" rel=\"stylesheet\">");
            // 引入 FontAwesome
            html.Append("<link rel=\"stylesheet\" href=\"https://cdnjs.cloudflare.com/ajax/libs/font-awesome/6.4.0/css/all.min.css\">");
            // 引入 Chart.js
            html.Append("<script src=\"https://cdn.jsdelivr.net/npm/chart.js\"></script>");
            // 引入 html2canvas 用于截图导出
            html.Append("<script src=\"https://cdn.jsdelivr.net/npm/html2canvas@1.4.1/dist/html2canvas.min.js\"></script>");
            
            AppendStyles(html);
            
            html.Append("</head><body>");
            
            AppendBody(html);
            AppendScripts(html);
            
            html.Append("</body></html>");
            
            return html.ToString();
        }

        private static void AppendStyles(StringBuilder html)
        {
            html.Append("<style>");
            html.Append(":root {");
            html.Append("  --bg-color: #f3f4f6;");
            html.Append("  --card-bg: #ffffff;");
            html.Append("  --text-color: #1f2937;");
            html.Append("  --text-secondary: #6b7280;");
            html.Append("  --primary-color: #3b82f6;");
            html.Append("  --primary-hover: #2563eb;");
            html.Append("  --success-color: #10b981;");
            html.Append("  --danger-color: #ef4444;");
            html.Append("  --border-color: #e5e7eb;");
            html.Append("  --shadow: 0 1px 3px 0 rgba(0, 0, 0, 0.1), 0 1px 2px 0 rgba(0, 0, 0, 0.06);");
            html.Append("}");

            html.Append("@media (prefers-color-scheme: dark) {");
            html.Append("  :root {");
            html.Append("    --bg-color: #111827;");
            html.Append("    --card-bg: #1f2937;");
            html.Append("    --text-color: #f9fafb;");
            html.Append("    --text-secondary: #9ca3af;");
            html.Append("    --primary-color: #60a5fa;");
            html.Append("    --primary-hover: #3b82f6;");
            html.Append("    --success-color: #34d399;");
            html.Append("    --danger-color: #f87171;");
            html.Append("    --border-color: #374151;");
            html.Append("  }");
            html.Append("}");

            html.Append("* { margin: 0; padding: 0; box-sizing: border-box; }");
            html.Append("body { font-family: -apple-system, BlinkMacSystemFont, 'Segoe UI', Roboto, 'Helvetica Neue', Arial, sans-serif; background-color: var(--bg-color); color: var(--text-color); line-height: 1.5; transition: background-color 0.3s, color 0.3s; }");
            html.Append(".container { max-width: 1200px; margin: 0 auto; padding: 24px; }");
            
            html.Append(".header { display: flex; align-items: center; justify-content: space-between; margin-bottom: 32px; padding-bottom: 16px; border-bottom: 1px solid var(--border-color); }");
            html.Append(".header h1 { font-size: 24px; font-weight: 700; color: var(--text-color); display: flex; align-items: center; gap: 12px; }");
            html.Append(".header h1 i { color: var(--primary-color); }");
            html.Append(".header p { color: var(--text-secondary); font-size: 14px; }");
            
            html.Append(".layout { display: grid; grid-template-columns: 240px 1fr; gap: 24px; }");
            html.Append("@media (max-width: 768px) { .layout { grid-template-columns: 1fr; } }");
            
            html.Append(".sidebar { display: flex; flex-direction: column; gap: 8px; }");
            html.Append(".nav-btn { display: flex; align-items: center; gap: 12px; padding: 12px 16px; border-radius: 8px; border: none; background: transparent; color: var(--text-secondary); cursor: pointer; font-size: 15px; font-weight: 500; text-align: left; transition: all 0.2s; }");
            html.Append(".nav-btn:hover { background-color: var(--border-color); color: var(--text-color); }");
            html.Append(".nav-btn.active { background-color: var(--primary-color); color: #ffffff; }");
            html.Append(".nav-btn i { width: 20px; text-align: center; }");
            
            html.Append(".content-area { min-height: 500px; }");
            html.Append(".tab-content { display: none; animation: fadeIn 0.3s ease; }");
            html.Append(".tab-content.active { display: block; }");
            html.Append("@keyframes fadeIn { from { opacity: 0; transform: translateY(10px); } to { opacity: 1; transform: translateY(0); } }");

            html.Append(".card { background-color: var(--card-bg); border-radius: 12px; padding: 24px; box-shadow: var(--shadow); border: 1px solid var(--border-color); margin-bottom: 24px; }");
            html.Append(".card-header { display: flex; justify-content: space-between; align-items: center; margin-bottom: 20px; }");
            html.Append(".card-title { font-size: 18px; font-weight: 600; color: var(--text-color); display: flex; align-items: center; gap: 8px; }");
            
            html.Append(".stats-grid { display: grid; grid-template-columns: repeat(auto-fit, minmax(200px, 1fr)); gap: 20px; margin-bottom: 24px; }");
            html.Append(".stat-item { background-color: var(--bg-color); padding: 20px; border-radius: 10px; border: 1px solid var(--border-color); display: flex; flex-direction: column; gap: 8px; }");
            html.Append(".stat-label { font-size: 13px; color: var(--text-secondary); font-weight: 500; text-transform: uppercase; letter-spacing: 0.5px; }");
            html.Append(".stat-value { font-size: 28px; font-weight: 700; color: var(--text-color); }");
            html.Append(".stat-value.highlight { color: var(--primary-color); }");
            
            html.Append(".charts-grid { display: grid; grid-template-columns: repeat(2, 1fr); gap: 24px; margin-bottom: 24px; }");
            html.Append(".chart-container { position: relative; height: 300px; width: 100%; }");
            html.Append("@media (max-width: 768px) { .charts-grid { grid-template-columns: 1fr; } }");

            html.Append(".damage-list { display: flex; flex-direction: column; gap: 12px; max-height: 600px; overflow-y: auto; padding-right: 8px; }");
            html.Append(".damage-item { display: flex; justify-content: space-between; align-items: center; padding: 16px; background-color: var(--bg-color); border-radius: 8px; border-left: 4px solid var(--primary-color); }");
            html.Append(".damage-info { display: flex; flex-direction: column; gap: 4px; }");
            html.Append(".damage-source { font-weight: 500; color: var(--text-color); }");
            html.Append(".damage-time { font-size: 12px; color: var(--text-secondary); }");
            html.Append(".damage-amount { font-size: 18px; font-weight: 700; color: var(--danger-color); }");
            
            html.Append(".task-grid { display: grid; gap: 16px; }");
            html.Append(".task-card { background-color: var(--bg-color); border-radius: 10px; padding: 20px; border: 1px solid var(--border-color); transition: transform 0.2s; }");
            html.Append(".task-card:hover { transform: translateY(-2px); }");
            html.Append(".task-header-row { display: flex; justify-content: space-between; align-items: flex-start; margin-bottom: 12px; }");
            html.Append(".task-name { font-weight: 600; font-size: 16px; color: var(--text-color); }");
            html.Append(".task-tag { font-size: 12px; padding: 4px 8px; border-radius: 999px; font-weight: 500; background-color: var(--card-bg); border: 1px solid var(--border-color); }");
            html.Append(".task-desc { color: var(--text-secondary); font-size: 14px; margin-bottom: 16px; }");
            html.Append(".progress-container { height: 8px; background-color: var(--border-color); border-radius: 4px; overflow: hidden; margin-bottom: 8px; }");
            html.Append(".progress-bar { height: 100%; background-color: var(--primary-color); transition: width 0.3s ease; }");
            html.Append(".progress-text { font-size: 13px; color: var(--text-secondary); text-align: right; }");
            
            html.Append(".task-objectives { margin: 12px 0; padding: 12px; background-color: var(--card-bg); border: 1px solid var(--border-color); border-radius: 8px; font-size: 14px; line-height: 1.6; }");
            html.Append(".task-rewards { margin: 12px 0; padding: 12px; background-color: rgba(245, 158, 11, 0.1); border-radius: 8px; font-size: 14px; border-left: 3px solid #f59e0b; }");
            html.Append(".task-section-title { font-weight: 600; margin-bottom: 8px; display: flex; align-items: center; gap: 6px; }");
            html.Append(".task-actions { margin-top: 16px; display: flex; flex-wrap: wrap; gap: 10px; border-top: 1px solid var(--border-color); padding-top: 12px; }");
            
            html.Append(".input-group { margin-bottom: 16px; }");
            html.Append(".input-label { display: block; font-size: 14px; font-weight: 500; color: var(--text-color); margin-bottom: 8px; }");
            html.Append(".form-control { width: 100%; padding: 10px 12px; border-radius: 6px; border: 1px solid var(--border-color); background-color: var(--bg-color); color: var(--text-color); font-size: 14px; transition: border-color 0.2s; }");
            html.Append(".form-control:focus { outline: none; border-color: var(--primary-color); box-shadow: 0 0 0 3px rgba(59, 130, 246, 0.1); }");
            
            html.Append(".btn { display: inline-flex; align-items: center; gap: 8px; padding: 10px 20px; border-radius: 6px; border: none; font-weight: 500; font-size: 14px; cursor: pointer; transition: all 0.2s; }");
            html.Append(".btn-primary { background-color: var(--primary-color); color: #ffffff; }");
            html.Append(".btn-primary:hover { background-color: var(--primary-hover); }");
            html.Append(".btn-success { background-color: var(--success-color); color: #ffffff; }");
            html.Append(".btn-success:hover { opacity: 0.9; }");
            html.Append(".btn-danger { background-color: var(--danger-color); color: #ffffff; }");
            html.Append(".btn-danger:hover { opacity: 0.9; }");
            html.Append(".btn-sm { padding: 6px 12px; font-size: 12px; }");
            html.Append(".btn:disabled { opacity: 0.5; cursor: not-allowed; }");
            
            html.Append(".hotkey-list { display: grid; grid-template-columns: repeat(auto-fit, minmax(200px, 1fr)); gap: 16px; }");
            html.Append(".hotkey-item { display: flex; align-items: center; justify-content: space-between; background-color: var(--bg-color); padding: 12px 16px; border-radius: 8px; border: 1px solid var(--border-color); }");
            html.Append(".kbd { background-color: var(--card-bg); border: 1px solid var(--border-color); padding: 4px 8px; border-radius: 4px; font-family: monospace; font-weight: 600; font-size: 14px; color: var(--text-color); box-shadow: 0 2px 0 var(--border-color); }");
            
            html.Append("::-webkit-scrollbar { width: 8px; }");
            html.Append("::-webkit-scrollbar-track { background: transparent; }");
            html.Append("::-webkit-scrollbar-thumb { background-color: var(--border-color); border-radius: 4px; }");
            html.Append("::-webkit-scrollbar-thumb:hover { background-color: var(--text-secondary); }");
            
            html.Append(".empty-state { text-align: center; padding: 48px 0; color: var(--text-secondary); }");
            html.Append(".empty-state i { font-size: 48px; margin-bottom: 16px; opacity: 0.5; }");
            
            html.Append(".items-grid { display: grid; grid-template-columns: repeat(auto-fill, minmax(200px, 1fr)); gap: 16px; margin-top: 20px; }");
            html.Append(".item-card { background-color: var(--bg-color); border: 1px solid var(--border-color); border-radius: 12px; padding: 16px; display: flex; flex-direction: column; align-items: center; gap: 12px; transition: all 0.2s; cursor: pointer; position: relative; overflow: hidden; height: 100%; min-height: 280px; }");
            html.Append(".item-card:hover { transform: translateY(-4px); box-shadow: 0 10px 20px rgba(0,0,0,0.1); border-color: var(--primary-color); }");
            
            html.Append(".item-icon-wrapper { width: 80px; height: 80px; background-color: var(--card-bg); border-radius: 12px; display: flex; align-items: center; justify-content: center; border: 1px solid var(--border-color); margin-bottom: 8px; flex-shrink: 0; }");
            html.Append(".item-icon-wrapper img { max-width: 64px; max-height: 64px; object-fit: contain; }");
            html.Append(".item-icon-wrapper i { font-size: 32px; color: var(--text-secondary); opacity: 0.5; }");
            
            html.Append(".item-info { width: 100%; text-align: center; display: flex; flex-direction: column; flex: 1; }");
            html.Append(".item-name { font-weight: 600; font-size: 14px; color: var(--text-color); margin-bottom: 8px; line-height: 1.4; min-height: 40px; display: -webkit-box; -webkit-line-clamp: 2; -webkit-box-orient: vertical; overflow: hidden; }");
            html.Append(".item-desc { font-size: 12px; color: var(--text-secondary); margin-bottom: 12px; line-height: 1.5; height: 36px; overflow: hidden; display: -webkit-box; -webkit-line-clamp: 2; -webkit-box-orient: vertical; }");
            html.Append(".item-meta { display: flex; justify-content: center; gap: 6px; font-size: 11px; color: var(--text-secondary); margin-top: auto; flex-wrap: wrap; }");
            html.Append(".item-tag { font-size: 10px; padding: 4px 8px; border-radius: 6px; background-color: var(--border-color); color: var(--text-secondary); font-weight: 500; white-space: nowrap; }");
            
            html.Append(".item-price-tag { font-size: 12px; font-weight: 600; color: var(--success-color); display: flex; align-items: center; gap: 4px; justify-content: center; margin-top: 8px; }");
            
            // 搜索栏样式
            html.Append(".filter-bar { display: flex; gap: 12px; margin-bottom: 20px; align-items: center; flex-wrap: wrap; background-color: var(--bg-color); padding: 16px; border-radius: 12px; border: 1px solid var(--border-color); }");
            html.Append(".search-input-group { flex: 1; min-width: 240px; position: relative; }");
            html.Append(".search-input-group i { position: absolute; left: 12px; top: 50%; transform: translateY(-50%); color: var(--text-secondary); }");
            html.Append(".search-input-group input { padding-left: 36px; width: 100%; height: 42px; }"); 
            
            html.Append(".filter-select { min-width: 140px; height: 42px; border-radius: 8px; border: 1px solid var(--border-color); padding: 0 12px; font-size: 14px; color: var(--text-color); background-color: var(--card-bg); cursor: pointer; transition: border-color 0.2s; }");
            html.Append(".filter-select:focus { outline: none; border-color: var(--primary-color); }");
            
            // 品质颜色条
            html.Append(".quality-bar { position: absolute; left: 0; top: 0; bottom: 0; width: 4px; }");
            html.Append(".q-1 { background-color: #9ca3af; }"); // Common
            html.Append(".q-2 { background-color: #10b981; }"); // Uncommon
            html.Append(".q-3 { background-color: #3b82f6; }"); // Rare
            html.Append(".q-4 { background-color: #8b5cf6; }"); // Epic
            html.Append(".q-5 { background-color: #f59e0b; }"); // Legendary
            
            html.Append(".pagination { display: flex; align-items: center; justify-content: center; gap: 8px; margin-top: 24px; padding: 16px; background-color: var(--bg-color); border-radius: 12px; border: 1px solid var(--border-color); }");
            html.Append(".page-btn { width: 36px; height: 36px; border: 1px solid var(--border-color); border-radius: 8px; background-color: var(--card-bg); color: var(--text-color); cursor: pointer; display: flex; align-items: center; justify-content: center; transition: all 0.2s; }");
            html.Append(".page-btn:hover { border-color: var(--primary-color); color: var(--primary-color); }");
            html.Append(".page-btn:disabled { opacity: 0.5; cursor: not-allowed; }");
            html.Append(".page-info { font-size: 14px; color: var(--text-color); padding: 0 16px; min-width: 120px; text-align: center; }");
            html.Append(".page-size-select { height: 36px; border-radius: 8px; border: 1px solid var(--border-color); padding: 0 12px; font-size: 13px; color: var(--text-color); background-color: var(--card-bg); cursor: pointer; margin-left: 16px; }");
            
            // Toast Notification Styles
            html.Append(".toast-container { position: fixed; bottom: 24px; right: 24px; z-index: 1000; display: flex; flex-direction: column; gap: 12px; pointer-events: none; }");
            html.Append(".toast { background-color: var(--card-bg); border-radius: 8px; padding: 16px 20px; box-shadow: 0 4px 12px rgba(0, 0, 0, 0.15); border-left: 4px solid var(--primary-color); display: flex; align-items: center; gap: 12px; min-width: 300px; transform: translateX(120%); transition: transform 0.3s ease-out; pointer-events: auto; }");
            html.Append(".toast.show { transform: translateX(0); }");
            html.Append(".toast-icon { font-size: 20px; }");
            html.Append(".toast-content { flex: 1; }");
            html.Append(".toast-title { font-weight: 600; font-size: 14px; color: var(--text-color); margin-bottom: 2px; }");
            html.Append(".toast-message { font-size: 13px; color: var(--text-secondary); }");
            html.Append(".toast-close { cursor: pointer; color: var(--text-secondary); font-size: 14px; transition: color 0.2s; }");
            html.Append(".toast-close:hover { color: var(--text-color); }");
            html.Append(".toast-success { border-left-color: var(--success-color); }");
            html.Append(".toast-success .toast-icon { color: var(--success-color); }");
            html.Append(".toast-error { border-left-color: var(--danger-color); }");
            html.Append(".toast-error .toast-icon { color: var(--danger-color); }");
            html.Append(".toast-info { border-left-color: var(--primary-color); }");
            html.Append(".toast-info .toast-icon { color: var(--primary-color); }");
            html.Append(".toast-warning { border-left-color: var(--warning-color); }"); // Fixed warning color var usage
            
            //  Empty State 布局
            html.Append(".empty-state { display: flex; flex-direction: column; align-items: center; justify-content: center; text-align: center; padding: 60px 24px; color: var(--text-secondary); height: 100%; width: 100%; }");
            html.Append(".empty-state i { font-size: 48px; margin-bottom: 16px; opacity: 0.5; }");
            
            // Modal Dialog Styles
            html.Append(".modal-overlay { position: fixed; top: 0; left: 0; right: 0; bottom: 0; background: rgba(0,0,0,0.5); z-index: 2000; display: none; align-items: center; justify-content: center; }");
            html.Append(".modal-overlay.show { display: flex; }");
            html.Append(".modal { background: var(--card-bg); border-radius: 16px; padding: 24px; width: 90%; max-width: 400px; box-shadow: 0 20px 40px rgba(0,0,0,0.3); animation: modalIn 0.2s ease; }");
            html.Append("@keyframes modalIn { from { opacity: 0; transform: scale(0.9); } to { opacity: 1; transform: scale(1); } }");
            html.Append(".modal-header { display: flex; align-items: center; gap: 12px; margin-bottom: 20px; }");
            html.Append(".modal-icon { width: 48px; height: 48px; background: var(--bg-color); border-radius: 12px; display: flex; align-items: center; justify-content: center; border: 1px solid var(--border-color); }");
            html.Append(".modal-icon img { max-width: 40px; max-height: 40px; }");
            html.Append(".modal-title { font-size: 18px; font-weight: 600; color: var(--text-color); }");
            html.Append(".modal-subtitle { font-size: 13px; color: var(--text-secondary); margin-top: 2px; }");
            html.Append(".modal-body { margin-bottom: 24px; }");
            html.Append(".quantity-input-group { display: flex; align-items: center; gap: 12px; }");
            html.Append(".quantity-btn { width: 40px; height: 40px; border-radius: 8px; border: 1px solid var(--border-color); background: var(--bg-color); color: var(--text-color); font-size: 18px; cursor: pointer; transition: all 0.2s; display: flex; align-items: center; justify-content: center; }");
            html.Append(".quantity-btn:hover { background: var(--primary-color); color: #fff; border-color: var(--primary-color); }");
            html.Append(".quantity-input { flex: 1; height: 40px; text-align: center; font-size: 18px; font-weight: 600; border-radius: 8px; border: 1px solid var(--border-color); background: var(--bg-color); color: var(--text-color); }");
            html.Append(".quantity-input:focus { outline: none; border-color: var(--primary-color); }");
            html.Append(".modal-footer { display: flex; gap: 12px; }");
            html.Append(".modal-footer .btn { flex: 1; justify-content: center; }");
            html.Append(".btn-secondary { background: var(--bg-color); color: var(--text-color); border: 1px solid var(--border-color); }");
            html.Append(".btn-secondary:hover { background: var(--border-color); }");
            
            html.Append(".dropdown-item { display: flex; align-items: center; gap: 8px; width: 100%; padding: 10px 16px; border: none; background: transparent; color: var(--text-color); font-size: 14px; cursor: pointer; text-align: left; transition: background 0.2s; }");
            html.Append(".dropdown-item:hover { background: var(--bg-color); }");
            html.Append(".dropdown-item i { width: 16px; color: var(--text-secondary); }");
            
            html.Append("</style>");
        }

        private static void AppendBody(StringBuilder html)
        {
            html.Append("<div class=\"container\">");
            
            // Header
            html.Append("<header class=\"header\">");
            html.Append("<div><h1><i class=\"ri-dashboard-3-line\"></i> <span>OmniGauge</span></h1><p>游戏数据分析与物品管理助手</p></div>");
            html.Append("<div><span class=\"task-tag\"><i class=\"fa-solid fa-circle-check\" style=\"color: var(--success-color); margin-right: 6px;\"></i> 系统运行中</span></div>");
            html.Append("</header>");
            
            html.Append("<div class=\"layout\">");
            
            // Sidebar Navigation
            html.Append("<nav class=\"sidebar\">");
            html.Append("<button class=\"nav-btn active\" onclick=\"switchTab(0)\"><i class=\"fa-solid fa-chart-line\"></i> 伤害统计</button>");
            html.Append("<button class=\"nav-btn\" onclick=\"switchTab(1)\"><i class=\"fa-solid fa-gamepad\"></i> 游戏任务</button>");
            html.Append("<button class=\"nav-btn\" onclick=\"switchTab(2)\"><i class=\"fa-solid fa-boxes-stacked\"></i> 物品浏览</button>");
            html.Append("<div style=\"margin-top: 24px; padding: 0 16px; font-size: 12px; color: var(--text-secondary); font-weight: 600; text-transform: uppercase;\">快捷操作</div>");
            html.Append("<div style=\"padding: 16px;\">");
            html.Append("<div class=\"hotkey-item\" style=\"margin-bottom: 12px;\"><span style=\"font-size: 14px;\">显示/隐藏</span><span class=\"kbd\">T</span></div>");
            html.Append("<div class=\"hotkey-item\"><span style=\"font-size: 14px;\">读取数据</span><span class=\"kbd\">R</span></div>");
            html.Append("</div>");
            html.Append("</nav>");
            
            html.Append("<main class=\"content-area\">");
            
            AppendDamageTab(html);
            AppendGameTasksTab(html);
            AppendItemsTab(html);
            
            html.Append("</main>");
            html.Append("</div>"); // End layout
            html.Append("<div id=\"toastContainer\" class=\"toast-container\"></div>");
            
            // Item Add Modal
            html.Append("<div id=\"itemModal\" class=\"modal-overlay\" onclick=\"if(event.target===this)closeItemModal()\">");
            html.Append("<div class=\"modal\">");
            html.Append("<div class=\"modal-header\">");
            html.Append("<div class=\"modal-icon\" id=\"modalItemIcon\"></div>");
            html.Append("<div><div class=\"modal-title\" id=\"modalItemName\">物品名称</div><div class=\"modal-subtitle\" id=\"modalItemCategory\">分类</div></div>");
            html.Append("</div>");
            html.Append("<div class=\"modal-body\">");
            html.Append("<label class=\"input-label\">添加数量</label>");
            html.Append("<div class=\"quantity-input-group\">");
            html.Append("<button class=\"quantity-btn\" onclick=\"adjustQuantity(-10)\">-10</button>");
            html.Append("<button class=\"quantity-btn\" onclick=\"adjustQuantity(-1)\">-</button>");
            html.Append("<input type=\"number\" id=\"itemQuantity\" class=\"quantity-input\" value=\"1\" min=\"1\" max=\"9999\">");
            html.Append("<button class=\"quantity-btn\" onclick=\"adjustQuantity(1)\">+</button>");
            html.Append("<button class=\"quantity-btn\" onclick=\"adjustQuantity(10)\">+10</button>");
            html.Append("</div>");
            html.Append("</div>");
            html.Append("<div class=\"modal-footer\">");
            html.Append("<button class=\"btn btn-secondary\" onclick=\"closeItemModal()\"><i class=\"fa-solid fa-xmark\"></i> 取消</button>");
            html.Append("<button class=\"btn btn-primary\" onclick=\"confirmAddItem()\"><i class=\"fa-solid fa-plus\"></i> 添加物品</button>");
            html.Append("</div>");
            html.Append("</div>");
            html.Append("</div>");

            // Confirm Modal
            html.Append("<div id=\"confirmModal\" class=\"modal-overlay\" onclick=\"if(event.target===this)closeConfirmModal()\">");
            html.Append("<div class=\"modal\">");
            html.Append("<div class=\"modal-header\" style=\"margin-bottom: 12px;\">");
            html.Append("<div><div class=\"modal-title\" id=\"confirmTitle\" style=\"font-size: 20px;\">确认操作</div></div>");
            html.Append("</div>");
            html.Append("<div class=\"modal-body\">");
            html.Append("<p id=\"confirmMessage\" style=\"font-size: 16px; color: var(--text-secondary); line-height: 1.6;\"></p>");
            html.Append("</div>");
            html.Append("<div class=\"modal-footer\">");
            html.Append("<button class=\"btn btn-secondary\" onclick=\"closeConfirmModal()\"><i class=\"fa-solid fa-xmark\"></i> 取消</button>");
            html.Append("<button class=\"btn btn-primary\" id=\"confirmBtn\"><i class=\"fa-solid fa-check\"></i> 确定</button>");
            html.Append("</div>");
            html.Append("</div>");
            html.Append("</div>");
            
            html.Append("</div>"); // End container
        }

        private static void AppendDamageTab(StringBuilder html)
        {
            html.Append("<div id=\"tab0\" class=\"tab-content active\">");
            
            // Stats Cards
            html.Append("<div class=\"stats-grid\">");
            html.Append("<div class=\"stat-item\"><span class=\"stat-label\">总伤害</span><span class=\"stat-value highlight\" id=\"totalDamage\">0</span></div>");
            html.Append("<div class=\"stat-item\"><span class=\"stat-label\">最大单次伤害</span><span class=\"stat-value\" id=\"maxDamage\">0</span></div>");
            html.Append("<div class=\"stat-item\"><span class=\"stat-label\">命中次数</span><span class=\"stat-value\" id=\"hitCount\">0</span></div>");
            html.Append("<div class=\"stat-item\"><span class=\"stat-label\">平均伤害</span><span class=\"stat-value\" id=\"avgDamage\">0</span></div>");
            html.Append("</div>");
            
            // Charts
            html.Append("<div class=\"card\">");
            html.Append("<div class=\"card-header\"><div class=\"card-title\"><i class=\"fa-solid fa-chart-area\"></i> 伤害分析</div></div>");
            html.Append("<div class=\"charts-grid\">");
            html.Append("<div class=\"chart-container\"><canvas id=\"dpsChart\"></canvas></div>");
            html.Append("<div class=\"chart-container\"><canvas id=\"weaponPieChart\"></canvas></div>");
            html.Append("</div>");
            html.Append("</div>");
            
            // Damage Log
            html.Append("<div class=\"card\">");
            html.Append("<div class=\"card-header\">");
            html.Append("<div class=\"card-title\"><i class=\"fa-solid fa-clock-rotate-left\"></i> 伤害日志</div>");
            html.Append("<div style=\"display: flex; gap: 8px;\">");
            html.Append("<div class=\"dropdown\" style=\"position: relative;\">");
            html.Append("<button class=\"btn btn-primary btn-sm\" onclick=\"toggleExportMenu()\"><i class=\"fa-solid fa-download\"></i> 导出 <i class=\"fa-solid fa-caret-down\"></i></button>");
            html.Append("<div id=\"exportMenu\" class=\"dropdown-menu\" style=\"display: none; position: absolute; right: 0; top: 100%; margin-top: 4px; background: var(--card-bg); border: 1px solid var(--border-color); border-radius: 8px; box-shadow: 0 4px 12px rgba(0,0,0,0.15); z-index: 100; min-width: 160px;\">");
            html.Append("<button class=\"dropdown-item\" onclick=\"exportDamageCSV()\"><i class=\"fa-solid fa-file-csv\"></i> CSV 表格</button>");
            html.Append("<button class=\"dropdown-item\" onclick=\"exportDamageJSON()\"><i class=\"fa-solid fa-file-code\"></i> JSON 数据</button>");
            html.Append("<button class=\"dropdown-item\" onclick=\"exportDamageHTML()\"><i class=\"fa-solid fa-file-lines\"></i> HTML 报告</button>");
            html.Append("<button class=\"dropdown-item\" onclick=\"exportDamageScreenshot()\"><i class=\"fa-solid fa-image\"></i> 精美图片</button>");
            html.Append("</div>");
            html.Append("</div>");
            html.Append("<button class=\"btn btn-danger btn-sm\" onclick=\"clearDamageLog()\"><i class=\"fa-solid fa-trash\"></i> 清空记录</button>");
            html.Append("</div>");
            html.Append("</div>");
            html.Append("<div class=\"damage-list\" id=\"damageLog\">");
            html.Append("<div class=\"empty-state\"><i class=\"fa-solid fa-clipboard\"></i><p>暂无伤害记录</p></div>");
            html.Append("</div>");
            html.Append("</div>");
            
            html.Append("</div>");
        }

        private static void AppendGameTasksTab(StringBuilder html)
        {
            html.Append("<div id=\"tab1\" class=\"tab-content\">");
            html.Append("<div class=\"card\">");
            html.Append("<div class=\"card-header\">");
            html.Append("<div class=\"card-title\"><i class=\"fa-solid fa-scroll\"></i> 游戏内任务</div>");
            html.Append("<button class=\"btn btn-primary btn-sm\" onclick=\"loadGameTasks()\"><i class=\"fa-solid fa-sync\"></i> 刷新任务列表</button>");
            html.Append("</div>");
            
            html.Append("<div class=\"filter-bar\">");
            html.Append("<div class=\"search-input-group\">");
            html.Append("<i class=\"fa-solid fa-search\"></i>");
            html.Append("<input type=\"text\" id=\"taskSearch\" class=\"form-control\" placeholder=\"搜索任务名称或ID...\" oninput=\"debouncedTaskSearch()\">");
            html.Append("</div>");
            html.Append("<select id=\"taskStatusFilter\" class=\"filter-select\" onchange=\"searchTasks()\">");
            html.Append("<option value=\"\">所有状态</option>");
            html.Append("<option value=\"进行中\">进行中</option>");
            html.Append("<option value=\"已完成\">已完成</option>");
            html.Append("<option value=\"可接取\">可接取</option>");
            html.Append("<option value=\"未激活\">未激活</option>");
            html.Append("<option value=\"已失败\">已失败</option>");
            html.Append("</select>");
            html.Append("</div>");
            
            html.Append("<div class=\"items-count\" id=\"tasksCount\" style=\"margin-left: 4px; font-weight: 500; margin-bottom: 16px;\">请点击刷新按钮加载任务</div>");
            
            html.Append("<div class=\"task-grid\" id=\"gameTasks\">");
            html.Append("<div class=\"empty-state\"><i class=\"fa-solid fa-search\"></i><p>暂无数据，请按 R 键读取游戏任务</p></div>");
            html.Append("</div>");
            
            html.Append("<div class=\"pagination\" id=\"tasksPagination\" style=\"display: none;\">");
            html.Append("<button class=\"page-btn\" onclick=\"goToTaskPage(1)\" title=\"首页\"><i class=\"fa-solid fa-angles-left\"></i></button>");
            html.Append("<button class=\"page-btn\" onclick=\"goToTaskPage(taskCurrentPage - 1)\" title=\"上一页\"><i class=\"fa-solid fa-angle-left\"></i></button>");
            html.Append("<span class=\"page-info\" id=\"taskPageInfo\">第 1 页</span>");
            html.Append("<button class=\"page-btn\" onclick=\"goToTaskPage(taskCurrentPage + 1)\" title=\"下一页\"><i class=\"fa-solid fa-angle-right\"></i></button>");
            html.Append("<button class=\"page-btn\" onclick=\"goToTaskPage(getTaskTotalPages())\" title=\"末页\"><i class=\"fa-solid fa-angles-right\"></i></button>");
            html.Append("<select id=\"taskPageSizeSelect\" class=\"page-size-select\" onchange=\"changeTaskPageSize(this.value)\">");
            html.Append("<option value=\"10\">10/页</option>");
            html.Append("<option value=\"20\">20/页</option>");
            html.Append("<option value=\"50\">50/页</option>");
            html.Append("<option value=\"100\">100/页</option>");
            html.Append("</select>");
            html.Append("</div>");
            
            html.Append("</div>");
            html.Append("</div>");
        }

        private static void AppendItemsTab(StringBuilder html)
        {
            html.Append("<div id=\"tab2\" class=\"tab-content\">");
            html.Append("<div class=\"card\">");
            html.Append("<div class=\"card-header\">");
            html.Append("<div class=\"card-title\"><i class=\"fa-solid fa-boxes-stacked\"></i> 物品数据库</div>");
            html.Append("<button class=\"btn btn-primary btn-sm\" onclick=\"loadItems()\"><i class=\"fa-solid fa-sync\"></i> 刷新数据</button>");
            html.Append("</div>");
            
            html.Append("<div class=\"filter-bar\">");
            html.Append("<div class=\"search-input-group\">");
            html.Append("<i class=\"fa-solid fa-search\"></i>");
            html.Append("<input type=\"text\" id=\"itemSearch\" class=\"form-control\" placeholder=\"搜索物品ID或名称...\" oninput=\"debouncedSearch()\">");
            html.Append("</div>");
            
            html.Append("<select id=\"qualityFilter\" class=\"filter-select\" onchange=\"searchItems()\">");
            html.Append("<option value=\"\">所有品质</option>");
            html.Append("<option value=\"1\">普通 (Common)</option>");
            html.Append("<option value=\"2\">罕见 (Uncommon)</option>");
            html.Append("<option value=\"3\">稀有 (Rare)</option>");
            html.Append("<option value=\"4\">史诗 (Epic)</option>");
            html.Append("<option value=\"5\">传说 (Legendary)</option>");
            html.Append("</select>");
            
            html.Append("<select id=\"categoryFilter\" class=\"filter-select\" onchange=\"searchItems()\">");
            html.Append("<option value=\"\">所有分类</option>");
            html.Append("</select>");
            html.Append("</div>");
            
            html.Append("<div class=\"items-count\" id=\"itemsCount\" style=\"margin-left: 4px; font-weight: 500;\">请点击刷新按钮加载数据</div>");
            
            html.Append("<div class=\"items-grid\" id=\"itemsGrid\">");
            html.Append("<div class=\"empty-state\" style=\"padding: 60px 0;\"><i class=\"fa-solid fa-box-open\" style=\"font-size: 64px; margin-bottom: 24px;\"></i><p style=\"font-size: 16px;\">点击上方刷新按钮加载物品数据</p></div>");
            html.Append("</div>");
            
            html.Append("<div class=\"pagination\" id=\"itemsPagination\" style=\"display: none;\">");
            html.Append("<button class=\"page-btn\" onclick=\"goToPage(1)\" title=\"首页\"><i class=\"fa-solid fa-angles-left\"></i></button>");
            html.Append("<button class=\"page-btn\" onclick=\"goToPage(currentPage - 1)\" title=\"上一页\"><i class=\"fa-solid fa-angle-left\"></i></button>");
            html.Append("<span class=\"page-info\" id=\"pageInfo\">第 1 页</span>");
            html.Append("<button class=\"page-btn\" onclick=\"goToPage(currentPage + 1)\" title=\"下一页\"><i class=\"fa-solid fa-angle-right\"></i></button>");
            html.Append("<button class=\"page-btn\" onclick=\"goToPage(getTotalPages())\" title=\"末页\"><i class=\"fa-solid fa-angles-right\"></i></button>");
            html.Append("<select id=\"pageSizeSelect\" class=\"page-size-select\" onchange=\"changePageSize(this.value)\">");
            html.Append("<option value=\"50\">50/页</option>");
            html.Append("<option value=\"100\">100/页</option>");
            html.Append("<option value=\"200\">200/页</option>");
            html.Append("<option value=\"500\">500/页</option>");
            html.Append("</select>");
            html.Append("</div>");
            
            html.Append("</div>");
            html.Append("</div>");
        }

        private static void AppendScripts(StringBuilder html)
        {
            html.Append("<script>");
            
            // Toast Notification System
            html.Append("function showToast(title, message, type) {");
            html.Append("type = type || 'info';");
            html.Append("var container = document.getElementById('toastContainer');");
            html.Append("var toast = document.createElement('div');");
            html.Append("toast.className = 'toast toast-' + type;");
            html.Append("var iconClass = 'fa-info-circle';");
            html.Append("if(type === 'success') iconClass = 'fa-check-circle';");
            html.Append("if(type === 'error') iconClass = 'fa-exclamation-circle';");
            html.Append("if(type === 'warning') iconClass = 'fa-exclamation-triangle';");
            html.Append("toast.innerHTML = '<div class=\"toast-icon\"><i class=\"fa-solid ' + iconClass + '\"></i></div>' +");
            html.Append("'<div class=\"toast-content\"><div class=\"toast-title\">' + title + '</div>' +");
            html.Append("'<div class=\"toast-message\">' + message + '</div></div>' +");
            html.Append("'<div class=\"toast-close\" onclick=\"this.parentElement.remove()\"><i class=\"fa-solid fa-xmark\"></i></div>';");
            html.Append("container.appendChild(toast);");
            html.Append("toast.offsetHeight;");
            html.Append("toast.classList.add('show');");
            html.Append("setTimeout(function() { toast.classList.remove('show'); setTimeout(function() { toast.remove(); }, 300); }, 3000);");
            html.Append("}");
            
            AppendDataInitialization(html);
            AppendTabFunctions(html);
            AppendDamageFunctions(html);
            AppendTaskFunctions(html);
            AppendItemFunctions(html);
            
            html.Append("loadGameTasks();");
            html.Append("loadDamageData();");
            html.Append("initCharts();");
            html.Append("setInterval(loadDamageData, 2000);");
            html.Append("setInterval(function() { loadGameTasks(true); }, 5000);");
            html.Append("window.switchTab = switchTab;");
            html.Append("window.initCharts = initCharts;");
            html.Append("window.clearDamageLog = clearDamageLog;");
            html.Append("window.toggleExportMenu = toggleExportMenu;");
            html.Append("window.exportDamageCSV = exportDamageCSV;");
            html.Append("window.exportDamageJSON = exportDamageJSON;");
            html.Append("window.exportDamageHTML = exportDamageHTML;");
            html.Append("window.exportDamageScreenshot = exportDamageScreenshot;");
            html.Append("window.completeGameTask = completeGameTask;");
            html.Append("window.resetGameTask = resetGameTask;");
            html.Append("window.activateTask = activateTask;");
            html.Append("window.forceCompleteTask = forceCompleteTask;");
            html.Append("window.showConfirmModal = showConfirmModal;");
            html.Append("window.closeConfirmModal = closeConfirmModal;");
            html.Append("window.loadItems = loadItems;");
            html.Append("window.searchItems = searchItems;");
            html.Append("window.goToPage = goToPage;");
            html.Append("window.changePageSize = changePageSize;");
            html.Append("window.getTotalPages = getTotalPages;");
            html.Append("window.openItemModal = openItemModal;");
            html.Append("window.closeItemModal = closeItemModal;");
            html.Append("window.adjustQuantity = adjustQuantity;");
            html.Append("window.confirmAddItem = confirmAddItem;");
            html.Append("window.searchTasks = searchTasks;");
            html.Append("window.goToTaskPage = goToTaskPage;");
            html.Append("window.changeTaskPageSize = changeTaskPageSize;");
            html.Append("window.getTaskTotalPages = getTaskTotalPages;");
            html.Append("window.addDamage = addDamage; window.loadGameTasks = loadGameTasks; window.loadDamageData = loadDamageData;");
            html.Append("</script>");
        }

        private static void AppendDataInitialization(StringBuilder html)
        {
            html.Append("let damageLog = []; let totalDamage = 0; let maxDamage = 0; let hitCount = 0;");
            html.Append("let dpsChart = null; let weaponPieChart = null;");
            html.Append("let allItems = []; let filteredItems = []; let currentPage = 1; let itemsPerPage = parseInt(localStorage.getItem('omni_itemsPerPage')) || 50;");
            html.Append("let allTasks = []; let filteredTasks = []; let taskCurrentPage = 1; let tasksPerPage = parseInt(localStorage.getItem('omni_tasksPerPage')) || 20;");
        }

        private static void AppendTabFunctions(StringBuilder html)
        {
            html.Append("function switchTab(index) {");
            html.Append("document.querySelectorAll('.nav-btn').forEach((btn, i) => { if(i === index) btn.classList.add('active'); else btn.classList.remove('active'); });");
            html.Append("document.querySelectorAll('.tab-content').forEach((content, i) => { if(i === index) content.classList.add('active'); else content.classList.remove('active'); });");
            html.Append("if(index === 0) { setTimeout(() => { if(dpsChart) dpsChart.resize(); if(weaponPieChart) weaponPieChart.resize(); }, 100); }");
            html.Append("}");
        }

        private static void AppendDamageFunctions(StringBuilder html)
        {
            html.Append("function initCharts() {");
            
            html.Append("const dpsCtx = document.getElementById('dpsChart').getContext('2d');");
            html.Append("dpsChart = new Chart(dpsCtx, { type: 'line', data: { labels: [], datasets: [{ label: '每秒伤害 (DPS)', data: [], borderColor: '#3b82f6', backgroundColor: 'rgba(59, 130, 246, 0.1)', tension: 0.4, fill: true }] }, options: { responsive: true, maintainAspectRatio: false, plugins: { legend: { display: false }, title: { display: true, text: '每秒伤害趋势' } }, scales: { y: { beginAtZero: true }, x: { display: false } } } });");
            
            html.Append("const pieCtx = document.getElementById('weaponPieChart').getContext('2d');");
            html.Append("weaponPieChart = new Chart(pieCtx, { type: 'doughnut', data: { labels: [], datasets: [{ data: [], backgroundColor: ['#3b82f6', '#10b981', '#f59e0b', '#ef4444', '#8b5cf6', '#ec4899'] }] }, options: { responsive: true, maintainAspectRatio: false, plugins: { legend: { position: 'right' }, title: { display: true, text: '武器伤害占比' } } } });");
            
            html.Append("}");

            html.Append("function loadDamageData() {");
            html.Append("fetch('/api/damage/stats').then(r => r.json()).then(data => {");
            html.Append("  totalDamage = data.totalDamage || 0;");
            html.Append("  maxDamage = data.maxDamage || 0;");
            html.Append("  hitCount = data.hitCount || 0;");
            html.Append("  updateDamageStats();");
            html.Append("}).catch(err => console.error('[OmniGauge] Failed to load damage stats:', err));");
            
            html.Append("fetch('/api/damage').then(r => r.json()).then(data => {");
            html.Append("  damageLog = data.map(d => ({value: d.damage, source: d.weapon, time: d.time, damageType: d.damageType, target: d.target, isCrit: d.isCrit}));");
            html.Append("  updateDamageLog();");
            html.Append("  updateCharts();");
            html.Append("}).catch(err => console.error('[OmniGauge] Failed to load damage log:', err));");
            html.Append("}");
            
            html.Append("function updateCharts() {");
            html.Append("if(!dpsChart || !weaponPieChart) return;");
            html.Append("const now = new Date();");
            html.Append("const timeWindow = 30;");
            html.Append("const labels = [];");
            html.Append("const dataPoints = [];");
            html.Append("for(let i=0; i<timeWindow; i++) { labels.push(''); dataPoints.push(0); }");
            html.Append("if(damageLog.length > 0) {");
            html.Append("  const recent = damageLog.slice(-100);");
            html.Append("  const lastLog = recent[recent.length-1];");
            html.Append("  if(lastLog) { dataPoints[dataPoints.length-1] = lastLog.value; }"); 
            html.Append("}");
            html.Append("const weaponDamage = {};");
            html.Append("damageLog.forEach(log => {");
            html.Append("  const source = log.source || 'Unknown';");
            html.Append("  weaponDamage[source] = (weaponDamage[source] || 0) + log.value;");
            html.Append("});");
            html.Append("weaponPieChart.data.labels = Object.keys(weaponDamage);");
            html.Append("weaponPieChart.data.datasets[0].data = Object.values(weaponDamage);");
            html.Append("weaponPieChart.update();");
            html.Append("dpsChart.data.labels = Array.from({length: damageLog.length > 0 ? damageLog.length : 1}, (_, i) => i + 1).slice(-50);");
            html.Append("dpsChart.data.datasets[0].data = damageLog.map(d => d.value).slice(-50);");
            html.Append("dpsChart.update();");
            html.Append("}");
            
            html.Append("function addDamage(value, source) {");
            html.Append("damageLog.push({ value, source, time: new Date().toLocaleTimeString() });");
            html.Append("totalDamage += value; hitCount++; if (value > maxDamage) maxDamage = value;");
            html.Append("updateDamageStats(); updateDamageLog(); updateCharts();");
            html.Append("}");
            
            html.Append("function updateDamageStats() {");
            html.Append("document.getElementById('totalDamage').textContent = totalDamage.toFixed(0);");
            html.Append("document.getElementById('maxDamage').textContent = maxDamage.toFixed(0);");
            html.Append("document.getElementById('hitCount').textContent = hitCount;");
            html.Append("document.getElementById('avgDamage').textContent = hitCount > 0 ? (totalDamage / hitCount).toFixed(1) : '0';");
            html.Append("}");
            
            html.Append("function updateDamageLog() {");
            html.Append("const logElement = document.getElementById('damageLog');");
            html.Append("if (damageLog.length === 0) { logElement.innerHTML = '<div class=\"empty-state\"><i class=\"fa-solid fa-clipboard\"></i><p>暂无伤害记录</p></div>'; return; }");
            html.Append("const recentLogs = damageLog.slice(-50).reverse();");
            html.Append("logElement.innerHTML = recentLogs.map(log => {");
            html.Append("  const critBadge = log.isCrit ? '<span style=\"color: var(--danger-color); font-weight: bold; margin-left: 4px;\">暴击</span>' : '';");
            html.Append("  const targetInfo = log.target ? ` → ${log.target}` : '';");
            html.Append("  const damageTypeText = log.damageType ? ` [${log.damageType}]` : '';");
            html.Append("  return `<div class=\"damage-item\"><div class=\"damage-info\"><span class=\"damage-source\">${log.source || '未知'}${targetInfo}${damageTypeText}${critBadge}</span><span class=\"damage-time\">${log.time}</span></div><div class=\"damage-amount\">${log.value.toFixed(1)}</div></div>`;");
            html.Append("}).join('');");
            html.Append("}");
            
            html.Append("function clearDamageLog() {");
            html.Append("if (confirm('确定要清空所有伤害记录吗？')) { damageLog = []; totalDamage = 0; maxDamage = 0; hitCount = 0; updateDamageStats(); updateDamageLog(); updateCharts(); showToast('记录已清空', '所有伤害数据已重置', 'success'); }");
            html.Append("}");
            
            html.Append("function toggleExportMenu() {");
            html.Append("var menu = document.getElementById('exportMenu');");
            html.Append("menu.style.display = menu.style.display === 'none' ? 'block' : 'none';");
            html.Append("}");
            
            html.Append("document.addEventListener('click', function(e) {");
            html.Append("var menu = document.getElementById('exportMenu');");
            html.Append("if(!e.target.closest('.dropdown')) { menu.style.display = 'none'; }");
            html.Append("});");
            
            html.Append("function getExportTimestamp() { return new Date().toISOString().slice(0,19).replace(/:/g,'-'); }");
            html.Append("function getExportMeta() { return { exportTime: new Date().toLocaleString('zh-CN'), source: 'OmniGauge v1.2', totalRecords: damageLog.length, sessionStart: damageLog.length > 0 ? damageLog[0].time : 'N/A', sessionEnd: damageLog.length > 0 ? damageLog[damageLog.length-1].time : 'N/A' }; }");
            
            html.Append("function exportDamageCSV() {");
            html.Append("toggleExportMenu();");
            html.Append("if(damageLog.length === 0) { showToast('导出失败', '没有伤害记录可导出', 'warning'); return; }");
            html.Append("var meta = getExportMeta();");
            html.Append("var csvContent = '\\uFEFF';");
            html.Append("csvContent += '# OmniGauge 伤害数据导出\\n';");
            html.Append("csvContent += '# 导出时间: ' + meta.exportTime + '\\n';");
            html.Append("csvContent += '# 数据来源: ' + meta.source + '\\n';");
            html.Append("csvContent += '# 记录时间段: ' + meta.sessionStart + ' 至 ' + meta.sessionEnd + '\\n';");
            html.Append("csvContent += '# 总记录数: ' + meta.totalRecords + '\\n\\n';");
            html.Append("csvContent += '序号,时间,来源,目标,伤害类型,伤害值,是否暴击\\n';");
            html.Append("damageLog.forEach(function(log, idx) {");
            html.Append("  var row = [(idx+1), log.time, log.source || '未知', log.target || '未知', log.damageType || '普通', log.value.toFixed(2), log.isCrit ? '是' : '否'].join(',');");
            html.Append("  csvContent += row + '\\n';");
            html.Append("});");
            html.Append("csvContent += '\\n# 统计汇总\\n';");
            html.Append("csvContent += '总伤害,' + totalDamage.toFixed(2) + '\\n';");
            html.Append("csvContent += '最大单次伤害,' + maxDamage.toFixed(2) + '\\n';");
            html.Append("csvContent += '命中次数,' + hitCount + '\\n';");
            html.Append("csvContent += '平均伤害,' + (hitCount > 0 ? (totalDamage / hitCount).toFixed(2) : 0) + '\\n';");
            html.Append("csvContent += 'DPS(估算),' + (damageLog.length > 1 ? (totalDamage / ((new Date(damageLog[damageLog.length-1].timestamp) - new Date(damageLog[0].timestamp)) / 1000)).toFixed(2) : 'N/A') + '\\n';");
            html.Append("var blob = new Blob([csvContent], {type: 'text/csv;charset=utf-8'});");
            html.Append("var link = document.createElement('a');");
            html.Append("link.href = URL.createObjectURL(blob);");
            html.Append("link.download = 'OmniGauge_伤害记录_' + getExportTimestamp() + '.csv';");
            html.Append("link.click();");
            html.Append("showToast('导出成功', '已导出 ' + damageLog.length + ' 条CSV记录', 'success');");
            html.Append("}");
            
            html.Append("function exportDamageJSON() {");
            html.Append("toggleExportMenu();");
            html.Append("if(damageLog.length === 0) { showToast('导出失败', '没有伤害记录可导出', 'warning'); return; }");
            html.Append("var meta = getExportMeta();");
            html.Append("var exportData = {");
            html.Append("  meta: meta,");
            html.Append("  statistics: { totalDamage: totalDamage, maxDamage: maxDamage, hitCount: hitCount, avgDamage: hitCount > 0 ? totalDamage / hitCount : 0 },");
            html.Append("  records: damageLog");
            html.Append("};");
            html.Append("var blob = new Blob([JSON.stringify(exportData, null, 2)], {type: 'application/json'});");
            html.Append("var link = document.createElement('a');");
            html.Append("link.href = URL.createObjectURL(blob);");
            html.Append("link.download = 'OmniGauge_伤害记录_' + getExportTimestamp() + '.json';");
            html.Append("link.click();");
            html.Append("showToast('导出成功', '已导出JSON数据', 'success');");
            html.Append("}");
            
            html.Append("function exportDamageHTML() {");
            html.Append("toggleExportMenu();");
            html.Append("if(damageLog.length === 0) { showToast('导出失败', '没有伤害记录可导出', 'warning'); return; }");
            html.Append("var meta = getExportMeta();");
            html.Append("var avgDmg = hitCount > 0 ? (totalDamage / hitCount).toFixed(2) : 0;");
            html.Append("var critCount = 0; damageLog.forEach(function(l){ if(l.isCrit) critCount++; });");
            html.Append("var critRate = hitCount > 0 ? ((critCount/hitCount)*100).toFixed(1) : 0;");
            html.Append("var parseTime = function(t) { var p = t.split(':'); return parseInt(p[0])*3600 + parseInt(p[1])*60 + parseInt(p[2]); };");
            html.Append("var duration = 0; if(damageLog.length > 1) { var t1 = parseTime(damageLog[0].time); var t2 = parseTime(damageLog[damageLog.length-1].time); duration = t2 - t1; if(duration < 0) duration += 86400; }");
            html.Append("var dps = duration > 0 ? (totalDamage / duration).toFixed(1) : totalDamage.toFixed(1);");
            html.Append("var weaponStats = {};");
            html.Append("damageLog.forEach(function(log) { var src = log.source || '未知'; weaponStats[src] = (weaponStats[src] || 0) + log.value; });");
            html.Append("var weaponRows = Object.keys(weaponStats).sort(function(a,b) { return weaponStats[b] - weaponStats[a]; }).map(function(k) {");
            html.Append("  var pct = ((weaponStats[k] / totalDamage) * 100).toFixed(1);");
            html.Append("  return '<tr><td><div style=\"display:flex;align-items:center;gap:8px\"><i class=\"ri-focus-3-line\" style=\"color:#9ca3af\"></i> ' + k + '</div></td><td>' + weaponStats[k].toFixed(0) + '</td><td><div style=\"display:flex;align-items:center;gap:8px\"><span>' + pct + '%</span><div style=\"flex:1;height:6px;background:#374151;border-radius:3px;min-width:60px\"><div style=\"width:' + pct + '%;height:100%;background:#60a5fa;border-radius:3px\"></div></div></div></td></tr>';");
            html.Append("}).join('');");
            html.Append("var logRows = damageLog.slice(-100).reverse().map(function(log, idx) {");
            html.Append("  var critIcon = log.isCrit ? '<i class=\"ri-flashlight-fill\" style=\"color:#ef4444;margin-right:4px\"></i>' : '';");
            html.Append("  var critClass = log.isCrit ? 'style=\"color:#ef4444;font-weight:bold;\"' : '';");
            html.Append("  return '<tr><td>' + (damageLog.length - idx) + '</td><td>' + log.time + '</td><td>' + (log.source || '未知') + '</td><td>' + (log.target || '-') + '</td><td ' + critClass + '>' + critIcon + log.value.toFixed(1) + '</td></tr>';");
            html.Append("}).join('');");
            html.Append("var htmlContent = '<!DOCTYPE html><html><head><meta charset=\"UTF-8\"><title>OmniGauge 伤害报告</title>';");
            html.Append("htmlContent += '<link href=\"https://cdn.jsdelivr.net/npm/remixicon@3.5.0/fonts/remixicon.css\" rel=\"stylesheet\">';");
            html.Append("htmlContent += '<style>*{margin:0;padding:0;box-sizing:border-box}body{font-family:-apple-system,BlinkMacSystemFont,\"Segoe UI\",Roboto,Helvetica,Arial,sans-serif;background:#111827;color:#f3f4f6;min-height:100vh;padding:40px}';");
            html.Append("htmlContent += '.container{max-width:1000px;margin:0 auto;position:relative}.header{margin-bottom:40px;border-bottom:1px solid #374151;padding-bottom:20px;display:flex;justify-content:space-between;align-items:end}';");
            html.Append("htmlContent += '.header h1{font-size:32px;color:#fff;display:flex;align-items:center;gap:12px}.header h1 i{color:#60a5fa}.meta-info{text-align:right;color:#9ca3af;font-size:13px;line-height:1.6}';");
            html.Append("htmlContent += '.stats-grid{display:grid;grid-template-columns:repeat(3,1fr);gap:20px;margin-bottom:40px}';");
            html.Append("htmlContent += '.stat-card{background:#1f2937;border-radius:12px;padding:24px;border:1px solid #374151;position:relative;overflow:hidden}';");
            html.Append("htmlContent += '.stat-card .icon-bg{position:absolute;right:15px;top:15px;width:48px;height:48px;border-radius:50%;background:rgba(255,255,255,0.05);display:flex;align-items:center;justify-content:center;font-size:24px;color:#60a5fa}';");
            html.Append("htmlContent += '.stat-card .value{font-size:32px;font-weight:700;color:#fff;margin-bottom:4px;position:relative;z-index:1}';");
            html.Append("htmlContent += '.stat-card .label{font-size:13px;color:#9ca3af;text-transform:uppercase;letter-spacing:1px;position:relative;z-index:1}';");
            html.Append("htmlContent += '.grid-2{display:grid;grid-template-columns:1fr 1.5fr;gap:24px;margin-bottom:40px}';");
            html.Append("htmlContent += '.section{background:#1f2937;border-radius:12px;padding:24px;border:1px solid #374151}.section-header{font-size:16px;font-weight:600;margin-bottom:20px;display:flex;align-items:center;gap:8px;color:#fff}';");
            html.Append("htmlContent += '.section-header i{color:#60a5fa}table{width:100%;border-collapse:collapse}th{text-align:left;color:#9ca3af;font-weight:500;font-size:12px;padding:12px 8px;border-bottom:1px solid #374151;text-transform:uppercase}';");
            html.Append("htmlContent += 'td{padding:12px 8px;border-bottom:1px solid #374151;font-size:14px;color:#d1d5db}tr:last-child td{border-bottom:none}tr:hover td{background:#374151}';");
            html.Append("htmlContent += '.watermark{text-align:center;color:#4b5563;font-size:12px;margin-top:40px;display:flex;align-items:center;justify-content:center;gap:6px}.watermark i{font-size:14px}</style></head>';");
            html.Append("htmlContent += '<body><div class=\"container\"><div class=\"header\"><div><h1><i class=\"ri-dashboard-3-line\"></i> OmniGauge <span style=\"font-size:14px;background:#374151;padding:4px 8px;border-radius:4px;color:#9ca3af;font-weight:normal;margin-left:10px\">v1.2</span></h1></div>';");
            html.Append("htmlContent += '<div class=\"meta-info\"><div>导出时间: ' + meta.exportTime + '</div><div>会话时长: ' + meta.sessionStart + ' - ' + meta.sessionEnd + '</div></div></div>';");
            html.Append("htmlContent += '<div class=\"stats-grid\">';");
            html.Append("htmlContent += '<div class=\"stat-card\"><div class=\"icon-bg\"><i class=\"ri-sword-fill\"></i></div><div class=\"value\">' + totalDamage.toFixed(0) + '</div><div class=\"label\">累计伤害</div></div>';");
            html.Append("htmlContent += '<div class=\"stat-card\"><div class=\"icon-bg\" style=\"color:#eab308\"><i class=\"ri-speed-fill\"></i></div><div class=\"value\">' + dps + '</div><div class=\"label\">秒伤 (DPS)</div></div>';");
            html.Append("htmlContent += '<div class=\"stat-card\"><div class=\"icon-bg\" style=\"color:#ef4444\"><i class=\"ri-trophy-fill\"></i></div><div class=\"value\">' + maxDamage.toFixed(0) + '</div><div class=\"label\">最高一击</div></div>';");
            html.Append("htmlContent += '<div class=\"stat-card\"><div class=\"icon-bg\" style=\"color:#10b981\"><i class=\"ri-crosshair-2-fill\"></i></div><div class=\"value\">' + hitCount + '</div><div class=\"label\">命中次数</div></div>';");
            html.Append("htmlContent += '<div class=\"stat-card\"><div class=\"icon-bg\" style=\"color:#a78bfa\"><i class=\"ri-bar-chart-fill\"></i></div><div class=\"value\">' + avgDmg + '</div><div class=\"label\">平均伤害</div></div>';");
            html.Append("htmlContent += '<div class=\"stat-card\"><div class=\"icon-bg\" style=\"color:#ec4899\"><i class=\"ri-flashlight-fill\"></i></div><div class=\"value\">' + critRate + '%</div><div class=\"label\">暴击率</div></div></div>';");
            html.Append("htmlContent += '<div class=\"grid-2\"><div class=\"section\"><div class=\"section-header\"><i class=\"ri-pie-chart-2-fill\"></i> 武器伤害占比</div><table><thead><tr><th>来源</th><th>数值</th><th>占比</th></tr></thead><tbody>' + weaponRows + '</tbody></table></div>';");
            html.Append("htmlContent += '<div class=\"section\"><div class=\"section-header\"><i class=\"ri-file-list-3-line\"></i> 详细伤害日志 (Top 100)</div><table><thead><tr><th>#</th><th>时间</th><th>来源</th><th>目标</th><th>数值</th></tr></thead><tbody>' + logRows + '</tbody></table></div></div>';");
            html.Append("htmlContent += '<div class=\"watermark\"><i class=\"ri-fingerprint-line\"></i> Generated by OmniGauge Analysis Tool</div></div></body></html>';");
            html.Append("var blob = new Blob([htmlContent], {type: 'text/html;charset=utf-8'});");
            html.Append("var link = document.createElement('a');");
            html.Append("link.href = URL.createObjectURL(blob);");
            html.Append("link.download = 'OmniGauge_HTML_' + getExportTimestamp() + '.html';");
            html.Append("link.click();");
            html.Append("showToast('导出成功', '已生成HTML报告', 'success');");
            html.Append("}");
            
            html.Append("function exportDamageScreenshot() {");
            html.Append("toggleExportMenu();");
            html.Append("if(typeof html2canvas === 'undefined') { showToast('导出失败', '截图库未加载', 'error'); return; }");
            html.Append("showToast('正在生成', '精美图片生成中...', 'info');");
            html.Append("var avgDmg = hitCount > 0 ? (totalDamage / hitCount).toFixed(1) : 0;");
            html.Append("var critCount = 0; damageLog.forEach(function(l){ if(l.isCrit) critCount++; });");
            html.Append("var critRate = hitCount > 0 ? ((critCount/hitCount)*100).toFixed(1) : 0;");
            html.Append("var parseTime = function(t) { var p = t.split(':'); return parseInt(p[0])*3600 + parseInt(p[1])*60 + parseInt(p[2]); };");
            html.Append("var duration = 0; if(damageLog.length > 1) { var t1 = parseTime(damageLog[0].time); var t2 = parseTime(damageLog[damageLog.length-1].time); duration = t2 - t1; if(duration < 0) duration += 86400; }");
            html.Append("var dps = duration > 0 ? (totalDamage / duration).toFixed(1) : totalDamage.toFixed(1);");
            
            html.Append("var exportDiv = document.createElement('div');");
            html.Append("exportDiv.style.cssText = 'position:fixed;left:-9999px;top:0;width:900px;padding:60px;background:#111827;font-family:\"Segoe UI\",Roboto,Helvetica,Arial,sans-serif;color:#f3f4f6;box-sizing:border-box;';");
            html.Append("var weaponStats = {};");
            html.Append("damageLog.forEach(function(log) { var src = log.source || '未知'; weaponStats[src] = (weaponStats[src] || 0) + log.value; });");
            html.Append("var topWeapons = Object.keys(weaponStats).sort(function(a,b) { return weaponStats[b] - weaponStats[a]; }).slice(0,5);");
            html.Append("var weaponBars = topWeapons.map(function(w) {");
            html.Append("  var pct = (weaponStats[w] / totalDamage * 100).toFixed(1);");
            html.Append("  var barWidth = Math.min(100, (weaponStats[w] / weaponStats[topWeapons[0]]) * 100);");
            html.Append("  return '<div style=\"margin-bottom:16px;\"><div style=\"display:flex;justify-content:space-between;margin-bottom:6px;font-size:14px;\"><span><i class=\"ri-focus-3-line\" style=\"color:#6b7280;margin-right:6px;vertical-align:middle\"></i>' + w + '</span><span style=\"color:#9ca3af\">' + weaponStats[w].toFixed(0) + ' <span style=\"font-size:12px;opacity:0.7\">(' + pct + '%)</span></span></div><div style=\"height:8px;background:#374151;border-radius:4px;overflow:hidden;\"><div style=\"width:' + barWidth + '%;height:100%;background:#60a5fa;border-radius:4px;\"></div></div></div>';");
            html.Append("}).join('');");
            html.Append("var recentLogs = damageLog.slice(-6).reverse().map(function(log) {");
            html.Append("  var critBadge = log.isCrit ? '<i class=\"ri-flashlight-fill\" style=\"color:#ef4444;margin-left:8px;vertical-align:middle\"></i>' : '';");
            html.Append("  var valColor = log.isCrit ? '#ef4444' : '#60a5fa';");
            html.Append("  return '<div style=\"display:flex;justify-content:space-between;align-items:center;padding:12px 0;border-bottom:1px solid #374151;\"><div style=\"display:flex;align-items:center\"><span style=\"color:#d1d5db;\">' + (log.source || '未知') + '</span><i class=\"ri-arrow-right-s-line\" style=\"font-size:14px;color:#6b7280;margin:0 8px;vertical-align:middle\"></i><span style=\"color:#9ca3af;\">' + (log.target || '敌人') + '</span>' + critBadge + '</div><span style=\"font-size:16px;font-weight:700;color:' + valColor + ';\">' + log.value.toFixed(0) + '</span></div>';");
            html.Append("}).join('');");
            html.Append("exportDiv.innerHTML = '<div style=\"display:flex;justify-content:space-between;align-items:start;margin-bottom:40px;border-bottom:1px solid #374151;padding-bottom:20px;\">';");
            html.Append("exportDiv.innerHTML += '<div><div style=\"font-size:36px;font-weight:800;color:#fff;display:flex;align-items:center;gap:12px;margin-bottom:4px\"><i class=\"ri-dashboard-3-line\" style=\"color:#60a5fa;font-size:40px\"></i> OmniGauge</div><div style=\"font-size:14px;color:#9ca3af;letter-spacing:1px;text-transform:uppercase;padding-left:4px\">专业伤害分析报告</div></div>';");
            html.Append("exportDiv.innerHTML += '<div style=\"text-align:right;color:#6b7280;font-size:13px;line-height:1.5\"><div>' + new Date().toLocaleString(\"zh-CN\") + '</div><div>本次会话: ' + damageLog.length + ' 条记录</div></div></div>';");
            html.Append("exportDiv.innerHTML += '<div style=\"display:grid;grid-template-columns:repeat(3,1fr);gap:20px;margin-bottom:40px;\">';");
            html.Append("exportDiv.innerHTML += '<div style=\"background:#1f2937;border-radius:12px;padding:24px;border:1px solid #374151;text-align:center;position:relative\"><div style=\"position:absolute;top:15px;right:15px;width:40px;height:40px;border-radius:50%;background:rgba(96,165,250,0.1);display:flex;align-items:center;justify-content:center;color:#60a5fa\"><i class=\"ri-sword-fill\" style=\"font-size:20px\"></i></div><div style=\"font-size:32px;font-weight:700;color:#fff\">' + totalDamage.toFixed(0) + '</div><div style=\"font-size:12px;color:#9ca3af;text-transform:uppercase;margin-top:4px\">累计伤害</div></div>';");
            html.Append("exportDiv.innerHTML += '<div style=\"background:#1f2937;border-radius:12px;padding:24px;border:1px solid #374151;text-align:center;position:relative\"><div style=\"position:absolute;top:15px;right:15px;width:40px;height:40px;border-radius:50%;background:rgba(234,179,8,0.1);display:flex;align-items:center;justify-content:center;color:#eab308\"><i class=\"ri-speed-fill\" style=\"font-size:20px\"></i></div><div style=\"font-size:32px;font-weight:700;color:#fff\">' + dps + '</div><div style=\"font-size:12px;color:#9ca3af;text-transform:uppercase;margin-top:4px\">秒伤 (DPS)</div></div>';");
            html.Append("exportDiv.innerHTML += '<div style=\"background:#1f2937;border-radius:12px;padding:24px;border:1px solid #374151;text-align:center;position:relative\"><div style=\"position:absolute;top:15px;right:15px;width:40px;height:40px;border-radius:50%;background:rgba(239,68,68,0.1);display:flex;align-items:center;justify-content:center;color:#ef4444\"><i class=\"ri-trophy-fill\" style=\"font-size:20px\"></i></div><div style=\"font-size:32px;font-weight:700;color:#fff\">' + maxDamage.toFixed(0) + '</div><div style=\"font-size:12px;color:#9ca3af;text-transform:uppercase;margin-top:4px\">最高一击</div></div>';");
            html.Append("exportDiv.innerHTML += '<div style=\"background:#1f2937;border-radius:12px;padding:24px;border:1px solid #374151;text-align:center;position:relative\"><div style=\"position:absolute;top:15px;right:15px;width:40px;height:40px;border-radius:50%;background:rgba(16,185,129,0.1);display:flex;align-items:center;justify-content:center;color:#10b981\"><i class=\"ri-crosshair-2-fill\" style=\"font-size:20px\"></i></div><div style=\"font-size:32px;font-weight:700;color:#fff\">' + hitCount + '</div><div style=\"font-size:12px;color:#9ca3af;text-transform:uppercase;margin-top:4px\">命中次数</div></div>';");
            html.Append("exportDiv.innerHTML += '<div style=\"background:#1f2937;border-radius:12px;padding:24px;border:1px solid #374151;text-align:center;position:relative\"><div style=\"position:absolute;top:15px;right:15px;width:40px;height:40px;border-radius:50%;background:rgba(167,139,250,0.1);display:flex;align-items:center;justify-content:center;color:#a78bfa\"><i class=\"ri-bar-chart-fill\" style=\"font-size:20px\"></i></div><div style=\"font-size:32px;font-weight:700;color:#fff\">' + avgDmg + '</div><div style=\"font-size:12px;color:#9ca3af;text-transform:uppercase;margin-top:4px\">平均伤害</div></div>';");
            html.Append("exportDiv.innerHTML += '<div style=\"background:#1f2937;border-radius:12px;padding:24px;border:1px solid #374151;text-align:center;position:relative\"><div style=\"position:absolute;top:15px;right:15px;width:40px;height:40px;border-radius:50%;background:rgba(236,72,153,0.1);display:flex;align-items:center;justify-content:center;color:#ec4899\"><i class=\"ri-flashlight-fill\" style=\"font-size:20px\"></i></div><div style=\"font-size:32px;font-weight:700;color:#fff\">' + critRate + '<span style=\"font-size:16px\">%</span></div><div style=\"font-size:12px;color:#9ca3af;text-transform:uppercase;margin-top:4px\">暴击率</div></div></div>';");
            html.Append("exportDiv.innerHTML += '<div style=\"display:grid;grid-template-columns:1.2fr 1.5fr;gap:30px;margin-bottom:20px\">';");
            html.Append("exportDiv.innerHTML += '<div style=\"background:#1f2937;border-radius:12px;padding:24px;border:1px solid #374151\"><div style=\"font-size:16px;font-weight:600;margin-bottom:20px;display:flex;align-items:center;gap:10px;color:#fff;border-bottom:1px solid #374151;padding-bottom:10px\"><i class=\"ri-pie-chart-2-fill\" style=\"color:#60a5fa\"></i> <span>武器伤害统计</span></div>' + weaponBars + '</div>';");
            html.Append("exportDiv.innerHTML += '<div style=\"background:#1f2937;border-radius:12px;padding:24px;border:1px solid #374151\"><div style=\"font-size:16px;font-weight:600;margin-bottom:20px;display:flex;align-items:center;gap:10px;color:#fff;border-bottom:1px solid #374151;padding-bottom:10px\"><i class=\"ri-file-list-3-line\" style=\"color:#60a5fa\"></i> <span>最近伤害记录</span></div>' + recentLogs + '</div></div>';");
            html.Append("exportDiv.innerHTML += '<div style=\"text-align:right;margin-top:20px;color:#4b5563;font-size:12px;display:flex;justify-content:flex-end;align-items:center;gap:6px\"><i class=\"ri-fingerprint-line\"></i> 由 OmniGauge 分析工具生成</div>';");
            html.Append("document.body.appendChild(exportDiv);");
            html.Append("html2canvas(exportDiv, { backgroundColor: null, scale: 2, logging: false, allowTaint: true, useCORS: true }).then(function(canvas) {");
            html.Append("  document.body.removeChild(exportDiv);");
            html.Append("  var link = document.createElement('a');");
            html.Append("  link.download = 'OmniGauge_IMG_' + getExportTimestamp() + '.png';");
            html.Append("  link.href = canvas.toDataURL('image/png');");
            html.Append("  link.click();");
            html.Append("  showToast('导出成功', '精美图片已保存', 'success');");
            html.Append("}).catch(function(err) { document.body.removeChild(exportDiv); showToast('导出失败', err.message, 'error'); });");
            html.Append("}");
        }

        private static void AppendTaskFunctions(StringBuilder html)
        {
            html.Append("var lastTasksHash = '';");
            html.Append("function getTasksHash(data) { return data.map(t => t.id + ':' + t.status + ':' + t.progress).join('|'); }");
            html.Append("function loadGameTasks(silent) {");
            html.Append("const gameTasksElement = document.getElementById('gameTasks');");
            html.Append("const countEl = document.getElementById('tasksCount');");
            html.Append("if(!silent) { gameTasksElement.innerHTML = '<div class=\"empty-state\"><i class=\"fa-solid fa-spinner fa-spin\"></i><p>正在加载数据...</p></div>'; }");
            html.Append("fetch('/api/tasks').then(r => r.json()).then(data => {");
            html.Append("var newHash = getTasksHash(data);");
            html.Append("if(silent && newHash === lastTasksHash) return;");
            html.Append("lastTasksHash = newHash;");
            html.Append("if (data.length === 0) { ");
            html.Append("  allTasks = [];");
            html.Append("  gameTasksElement.innerHTML = '<div class=\"empty-state\"><i class=\"fa-solid fa-search\"></i><p>暂无数据，请按 R 键读取游戏任务</p></div>'; ");
            html.Append("  countEl.textContent = '暂无任务数据';");
            html.Append("  return; ");
            html.Append("}");
            html.Append("allTasks = data;");
            html.Append("searchTasks();");
            html.Append("if(!silent) { showToast('加载成功', '已读取 ' + data.length + ' 个游戏任务', 'success'); }");
            html.Append("}).catch(err => { ");
            html.Append("if(silent) return;");
            html.Append("console.error('[OmniGauge] Failed to load tasks:', err);");
            html.Append("gameTasksElement.innerHTML = '<div class=\"empty-state\"><i class=\"fa-solid fa-exclamation-triangle\"></i><p>加载失败: ' + err.message + '</p><p style=\"font-size: 12px; margin-top: 8px;\">请检查控制台日志</p></div>'; ");
            html.Append("showToast('加载失败', '无法读取游戏任务', 'error');");
            html.Append("});");
            html.Append("}");

            html.Append("function searchTasks() {");
            html.Append("const query = document.getElementById('taskSearch').value.toLowerCase();");
            html.Append("const status = document.getElementById('taskStatusFilter').value;");
            html.Append("const countEl = document.getElementById('tasksCount');");
            html.Append("if (allTasks.length === 0) return;");
            html.Append("filteredTasks = allTasks.filter(task => {");
            html.Append("  const matchesQuery = !query || task.id.toString().includes(query) || (task.name && task.name.toLowerCase().includes(query));");
            html.Append("  const matchesStatus = !status || task.status === status;");
            html.Append("  return matchesQuery && matchesStatus;");
            html.Append("});");
            html.Append("countEl.textContent = '找到 ' + filteredTasks.length + ' 个任务 (总数: ' + allTasks.length + ')';");
            html.Append("renderTasks(filteredTasks, true);");
            html.Append("}");

            html.Append("function renderTasks(tasks, resetPage) {");
            html.Append("if(resetPage) taskCurrentPage = 1;");
            html.Append("const gameTasksElement = document.getElementById('gameTasks');");
            html.Append("const pagination = document.getElementById('tasksPagination');");
            html.Append("if(tasks.length === 0) { gameTasksElement.innerHTML = '<div class=\"empty-state\"><i class=\"fa-solid fa-search\"></i><p>没有找到符合条件的任务</p></div>'; pagination.style.display = 'none'; return; }");
            html.Append("const totalPages = Math.ceil(tasks.length / tasksPerPage);");
            html.Append("if(taskCurrentPage > totalPages) taskCurrentPage = totalPages;");
            html.Append("const startIdx = (taskCurrentPage - 1) * tasksPerPage;");
            html.Append("const endIdx = Math.min(startIdx + tasksPerPage, tasks.length);");
            html.Append("const pageTasks = tasks.slice(startIdx, endIdx);");
            html.Append("let html = '';");
            html.Append("var statusColors = {'进行中': 'var(--primary-color)', '已完成': 'var(--success-color)', '可接取': '#f59e0b', '已失败': 'var(--danger-color)', '未激活': '#9ca3af'};");
            html.Append("pageTasks.forEach((task) => {");
            html.Append("var statusColor = statusColors[task.status] || 'var(--text-secondary)';");
            html.Append("html += '<div class=\"task-card\" style=\"border-left: 4px solid ' + statusColor + ';\">';");
            html.Append("html += '<div class=\"task-header-row\"><div class=\"task-name\">[' + task.id + '] ' + (task.name || '未命名任务') + '</div><div class=\"task-tag\" style=\"background: ' + statusColor + '; color: #fff; border: none;\">' + (task.status || 'Unknown') + '</div></div>';");
            html.Append("if(task.objectives) { ");
            html.Append("  var objHtml = task.objectives.replace(/\\n/g, '<br>');");
            html.Append("  html += '<div class=\"task-objectives\">';");
            html.Append("  html += '<div class=\"task-section-title\" style=\"color: var(--text-color);\"><i class=\"fa-solid fa-bullseye\"></i>目标</div>';");
            html.Append("  html += '<div style=\"color: var(--text-secondary);\">' + objHtml + '</div>';");
            html.Append("  html += '</div>';");
            html.Append("}");
            html.Append("if(task.rewards && task.rewards !== '无奖励') { ");
            html.Append("  var rewHtml = task.rewards.replace(/\\n/g, '<br>');");
            html.Append("  html += '<div class=\"task-rewards\">';");
            html.Append("  html += '<div class=\"task-section-title\" style=\"color: #f59e0b;\"><i class=\"fa-solid fa-gift\"></i>奖励</div>';");
            html.Append("  html += '<div style=\"color: var(--text-secondary);\">' + rewHtml + '</div>';");
            html.Append("  html += '</div>';");
            html.Append("}");
            html.Append("html += '<div style=\"font-size: 13px; color: var(--text-secondary); margin-top: 8px;\"><i class=\"fa-solid fa-tasks\" style=\"margin-right: 6px;\"></i>进度：' + (task.progress || '0/0') + '</div>';");
            html.Append("html += '<div class=\"task-actions\">';");
            html.Append("if(task.status === '未激活' || task.status === '可接取') { html += '<button class=\"btn\" style=\"background-color: #3b82f6; color: white; padding: 6px 12px; font-size: 0.9em;\" onclick=\"activateTask(' + task.id + ')\"><i class=\"fa-solid fa-play\" style=\"margin-right: 6px;\"></i>激活任务</button>'; }");
            html.Append("if(task.status === '进行中') { html += '<button class=\"btn\" style=\"background-color: #ef4444; color: white; padding: 6px 12px; font-size: 0.9em;\" onclick=\"forceCompleteTask(' + task.id + ')\"><i class=\"fa-solid fa-forward-fast\" style=\"margin-right: 6px;\"></i>强制完成</button>'; }");
            html.Append("if(task.canComplete) { html += '<button class=\"btn\" style=\"background-color: var(--success-color); color: white; padding: 6px 12px; font-size: 0.9em;\" onclick=\"completeGameTask(' + task.id + ')\"><i class=\"fa-solid fa-check\" style=\"margin-right: 6px;\"></i>完成任务</button>'; }");
            html.Append("if(task.status === '已完成') { html += '<button class=\"btn\" style=\"background-color: #f59e0b; color: white; padding: 6px 12px; font-size: 0.9em;\" onclick=\"resetGameTask(' + task.id + ')\"><i class=\"fa-solid fa-rotate-left\" style=\"margin-right: 6px;\"></i>重置任务</button>'; }");
            html.Append("html += '</div>';");
            html.Append("html += '</div>';");
            html.Append("});");
            html.Append("gameTasksElement.innerHTML = html;");
            html.Append("updateTaskPagination();");
            html.Append("}");

            html.Append("function getTaskTotalPages() { return Math.ceil(filteredTasks.length / tasksPerPage); }");

            html.Append("function updateTaskPagination() {");
            html.Append("var pagination = document.getElementById('tasksPagination');");
            html.Append("var pageInfo = document.getElementById('taskPageInfo');");
            html.Append("var totalPages = getTaskTotalPages();");
            html.Append("if(totalPages <= 1) { pagination.style.display = 'none'; return; }");
            html.Append("pagination.style.display = 'flex';");
            html.Append("var startIdx = (taskCurrentPage - 1) * tasksPerPage + 1;");
            html.Append("var endIdx = Math.min(taskCurrentPage * tasksPerPage, filteredTasks.length);");
            html.Append("pageInfo.textContent = '第 ' + taskCurrentPage + '/' + totalPages + ' 页 (' + startIdx + '-' + endIdx + ')';");
            html.Append("var btns = pagination.querySelectorAll('.page-btn');");
            html.Append("btns[0].disabled = taskCurrentPage === 1;");
            html.Append("btns[1].disabled = taskCurrentPage === 1;");
            html.Append("btns[2].disabled = taskCurrentPage === totalPages;");
            html.Append("btns[3].disabled = taskCurrentPage === totalPages;");
            html.Append("}");

            html.Append("function goToTaskPage(page) {");
            html.Append("var totalPages = getTaskTotalPages();");
            html.Append("if(page < 1) page = 1;");
            html.Append("if(page > totalPages) page = totalPages;");
            html.Append("if(page === taskCurrentPage) return;");
            html.Append("taskCurrentPage = page;");
            html.Append("renderTasks(filteredTasks, false);");
            html.Append("document.getElementById('gameTasks').scrollIntoView({behavior: 'smooth', block: 'start'});");
            html.Append("}");

            html.Append("function changeTaskPageSize(size) {");
            html.Append("tasksPerPage = parseInt(size);");
            html.Append("localStorage.setItem('omni_tasksPerPage', tasksPerPage);");
            html.Append("taskCurrentPage = 1;");
            html.Append("renderTasks(filteredTasks, false);");
            html.Append("document.getElementById('gameTasks').scrollIntoView({behavior: 'smooth', block: 'start'});");
            html.Append("}");
            
            html.Append("const debouncedTaskSearch = debounce(searchTasks, 300);");
            
            html.Append("document.addEventListener('DOMContentLoaded', function() {");
            html.Append("  var taskPageSizeSelect = document.getElementById('taskPageSizeSelect');");
            html.Append("  if(taskPageSizeSelect) taskPageSizeSelect.value = tasksPerPage;");
            html.Append("});");

            html.Append("function completeGameTask(id) {");
            html.Append("fetch('/api/tasks/complete-game', {");
            html.Append("  method: 'POST',");
            html.Append("  headers: {'Content-Type': 'application/x-www-form-urlencoded'},");
            html.Append("  body: new URLSearchParams({id: id})");
            html.Append("}).then(r => r.json()).then(data => {");
            html.Append("  if(data.success) {");
            html.Append("    loadGameTasks();");
            html.Append("    showToast('操作成功', '任务尝试完成', 'success');");
            html.Append("  } else {");
            html.Append("    showToast('操作失败', '无法完成此任务，可能条件未达成', 'warning');");
            html.Append("  }");
            html.Append("}).catch(err => showToast('错误', '请求失败', 'error'));");
            html.Append("}");
            
            html.Append("function showConfirmModal(title, message, onConfirm) {");
            html.Append("document.getElementById('confirmTitle').textContent = title;");
            html.Append("document.getElementById('confirmMessage').textContent = message;");
            html.Append("var btn = document.getElementById('confirmBtn');");
            html.Append("btn.onclick = function() { closeConfirmModal(); onConfirm(); };");
            html.Append("document.getElementById('confirmModal').classList.add('show');");
            html.Append("}");
            
            html.Append("function closeConfirmModal() {");
            html.Append("document.getElementById('confirmModal').classList.remove('show');");
            html.Append("}");

            html.Append("function resetGameTask(id) {");
            html.Append("showConfirmModal('重置任务', '确定要重置此任务吗？任务将从已完成状态恢复为进行中。', function() {");
            html.Append("fetch('/api/tasks/reset', {");
            html.Append("  method: 'POST',");
            html.Append("  headers: {'Content-Type': 'application/x-www-form-urlencoded'},");
            html.Append("  body: new URLSearchParams({id: id})");
            html.Append("}).then(r => r.json()).then(data => {");
            html.Append("  if(data.success) {");
            html.Append("    loadGameTasks();");
            html.Append("    showToast('操作成功', '任务已重置', 'success');");
            html.Append("  } else {");
            html.Append("    showToast('操作失败', '无法重置此任务', 'warning');");
            html.Append("  }");
            html.Append("}).catch(err => showToast('错误', '请求失败', 'error'));");
            html.Append("});");
            html.Append("}");
            
            html.Append("function activateTask(id) {");
            html.Append("fetch('/api/tasks/activate', {");
            html.Append("  method: 'POST',");
            html.Append("  headers: {'Content-Type': 'application/x-www-form-urlencoded'},");
            html.Append("  body: new URLSearchParams({id: id})");
            html.Append("}).then(r => r.json()).then(data => {");
            html.Append("  if(data.success) {");
            html.Append("    loadGameTasks();");
            html.Append("    showToast('操作成功', '任务已激活', 'success');");
            html.Append("  } else {");
            html.Append("    showToast('操作失败', '无法激活此任务（可能前置条件未满足）', 'warning');");
            html.Append("  }");
            html.Append("}).catch(err => showToast('错误', '请求失败', 'error'));");
            html.Append("}");
            
            html.Append("function forceCompleteTask(id) {");
            html.Append("showConfirmModal('强制完成', '确定要强制完成此任务吗？这将跳过所有任务目标。', function() {");
            html.Append("fetch('/api/tasks/force-complete', {");
            html.Append("  method: 'POST',");
            html.Append("  headers: {'Content-Type': 'application/x-www-form-urlencoded'},");
            html.Append("  body: new URLSearchParams({id: id})");
            html.Append("}).then(r => r.json()).then(data => {");
            html.Append("  if(data.success) {");
            html.Append("    loadGameTasks();");
            html.Append("    showToast('操作成功', '任务已强制完成', 'success');");
            html.Append("  } else {");
            html.Append("    showToast('操作失败', '无法强制完成此任务', 'warning');");
            html.Append("  }");
            html.Append("}).catch(err => showToast('错误', '请求失败', 'error'));");
            html.Append("});");
            html.Append("}");
        }

        private static void AppendItemFunctions(StringBuilder html)
        {
            html.Append("var itemLoadTimer = null;");
            html.Append("var selectedItemId = null;");
            html.Append("var categoryMap = {");
            html.Append("'Weapon': '武器', 'Weapons': '武器', 'Gun': '枪械', 'Guns': '枪械',");
            html.Append("'Melee': '近战武器', 'MeleeWeapon': '近战武器', 'Ranged': '远程武器',");
            html.Append("'Armor': '护甲', 'Armors': '护甲', 'Helmet': '头盔', 'Chest': '胸甲', 'Legs': '腿甲',");
            html.Append("'Consumable': '消耗品', 'Consumables': '消耗品', 'Food': '食物', 'Drink': '饮料',");
            html.Append("'Medicine': '药品', 'Medical': '医疗', 'Health': '生命',");
            html.Append("'Ammo': '弹药', 'Ammunition': '弹药', 'Bullet': '子弹',");
            html.Append("'Material': '材料', 'Materials': '材料', 'Resource': '资源', 'Resources': '资源',");
            html.Append("'Tool': '工具', 'Tools': '工具', 'Equipment': '装备',");
            html.Append("'Quest': '任务物品', 'QuestItem': '任务物品', 'Key': '钥匙', 'Keys': '钥匙',");
            html.Append("'Misc': '杂项', 'Miscellaneous': '杂项', 'Other': '其他', 'None': '未分类',");
            html.Append("'Attachment': '配件', 'Attachments': '配件', 'Mod': '改装件', 'Mods': '改装件',");
            html.Append("'Clothing': '服装', 'Clothes': '服装', 'Outfit': '套装',");
            html.Append("'Accessory': '饰品', 'Accessories': '饰品', 'Jewelry': '首饰',");
            html.Append("'Crafting': '制作材料', 'Component': '组件', 'Components': '组件',");
            html.Append("'Throwable': '投掷物', 'Grenade': '手雷', 'Explosive': '爆炸物',");
            html.Append("'Container': '容器', 'Storage': '存储', 'Bag': '背包', 'Backpack': '背包',");
            html.Append("'Currency': '货币', 'Money': '金钱', 'Valuable': '贵重品',");
            html.Append("'Document': '文档', 'Note': '笔记', 'Book': '书籍',");
            html.Append("'Junk': '垃圾', 'Scrap': '废料', 'Trash': '废物'");
            html.Append("};");
            html.Append("function translateCategory(cat) {");
            html.Append("if(!cat) return '未分类';");
            html.Append("return categoryMap[cat] || cat;");
            html.Append("}");
            html.Append("function loadItems() {");
            html.Append("var grid = document.getElementById('itemsGrid');");
            html.Append("var countEl = document.getElementById('itemsCount');");
            html.Append("grid.innerHTML = '<div class=\"empty-state\"><i class=\"fa-solid fa-spinner fa-spin\"></i><p>正在加载物品数据...</p></div>';");
            html.Append("fetch('/api/items').then(function(r) { return r.json(); }).then(function(data) {");
            html.Append("  if(data.loading) {");
            html.Append("    var progress = data.progress || 0;");
            html.Append("    countEl.textContent = data.message || '正在加载...';");
            html.Append("    grid.innerHTML = '<div class=\"empty-state\" style=\"padding: 60px 0;\"><i class=\"fa-solid fa-spinner fa-spin\" style=\"font-size: 48px; margin-bottom: 24px;\"></i><p style=\"font-size: 16px;\">正在加载物品数据...</p><div style=\"width: 200px; height: 8px; background: var(--border-color); border-radius: 4px; margin-top: 16px;\"><div style=\"width: ' + progress + '%; height: 100%; background: var(--primary-color); border-radius: 4px; transition: width 0.3s;\"></div></div><p style=\"font-size: 12px; margin-top: 8px;\">' + progress + '%</p></div>';");
            html.Append("    if(!itemLoadTimer) { itemLoadTimer = setTimeout(function() { itemLoadTimer = null; loadItems(); }, 500); }");
            html.Append("    return;");
            html.Append("  }");
            html.Append("  if(Array.isArray(data)) {");
            html.Append("    allItems = data;");
            html.Append("    updateCategoryFilter(data);");
            html.Append("    countEl.textContent = '共 ' + data.length + ' 个物品';");
            html.Append("    searchItems();");
            html.Append("    showToast('加载成功', '已加载 ' + data.length + ' 个物品', 'success');");
            html.Append("  }");
            html.Append("}).catch(function(err) {");
            html.Append("  console.error('[OmniGauge] Failed to load items:', err);");
            html.Append("  grid.innerHTML = '<div class=\"empty-state\"><i class=\"fa-solid fa-exclamation-triangle\"></i><p>加载失败</p></div>';");
            html.Append("  showToast('加载失败', '无法获取物品数据', 'error');");
            html.Append("});");
            html.Append("}");

            html.Append("function updateCategoryFilter(items) {");
            html.Append("var categories = new Set(items.map(function(i) { return i.category; }).filter(function(c) { return c && c !== 'None'; }));");
            html.Append("var select = document.getElementById('categoryFilter');");
            html.Append("var current = select.value;");
            html.Append("select.innerHTML = '<option value=\"\">所有分类</option>';");
            html.Append("Array.from(categories).sort().forEach(function(cat) {");
            html.Append("  var opt = document.createElement('option');");
            html.Append("  opt.value = cat;");
            html.Append("  opt.textContent = translateCategory(cat);");
            html.Append("  select.appendChild(opt);");
            html.Append("});");
            html.Append("select.value = current;");
            html.Append("}");

            html.Append("function renderItems(items, resetPage) {");
            html.Append("if(resetPage) currentPage = 1;");
            html.Append("filteredItems = items;");
            html.Append("var grid = document.getElementById('itemsGrid');");
            html.Append("var pagination = document.getElementById('itemsPagination');");
            html.Append("if(items.length === 0) { grid.innerHTML = '<div class=\"empty-state\" style=\"grid-column: 1/-1; padding: 60px 0;\"><i class=\"fa-solid fa-box-open\" style=\"font-size: 64px; margin-bottom: 24px;\"></i><p style=\"font-size: 16px;\">没有找到符合条件的物品</p></div>'; pagination.style.display = 'none'; return; }");
            html.Append("var totalPages = Math.ceil(items.length / itemsPerPage);");
            html.Append("if(currentPage > totalPages) currentPage = totalPages;");
            html.Append("var startIdx = (currentPage - 1) * itemsPerPage;");
            html.Append("var endIdx = Math.min(startIdx + itemsPerPage, items.length);");
            html.Append("var pageItems = items.slice(startIdx, endIdx);");
            html.Append("var html = '';");
            html.Append("pageItems.forEach(function(item) {");
            html.Append("  var qualityClass = 'q-' + Math.min(Math.max(item.quality, 1), 5);");
            html.Append("  var iconHtml = item.icon ? '<img src=\"data:image/png;base64,' + item.icon + '\" alt=\"\">' : '<i class=\"fa-solid fa-xmark\" style=\"font-size: 24px; opacity: 0.3;\"></i>';");
            html.Append("  var priceHtml = item.price > 0 ? '<div class=\"item-price-tag\"><i class=\"fa-solid fa-coins\"></i> ' + item.price + '</div>' : '';");
            html.Append("  var descHtml = item.description ? '<div class=\"item-desc\">' + item.description + '</div>' : '';");
            html.Append("  var catText = translateCategory(item.category);");
            html.Append("  html += '<div class=\"item-card\" onclick=\"openItemModal(' + item.id + ')\">';");
            html.Append("  html += '<div class=\"quality-bar ' + qualityClass + '\"></div>';");
            html.Append("  html += '<div class=\"item-icon-wrapper\">' + iconHtml + '</div>';");
            html.Append("  html += '<div class=\"item-info\">';");
            html.Append("  html += '<div class=\"item-name\">' + (item.displayName || item.name) + '</div>';");
            html.Append("  html += descHtml;");
            html.Append("  html += '<div class=\"item-meta\"><span class=\"item-tag\">ID: ' + item.id + '</span><span class=\"item-tag\">' + catText + '</span></div>';");
            html.Append("  html += priceHtml;");
            html.Append("  html += '</div></div>';");
            html.Append("});");
            html.Append("grid.innerHTML = html;");
            html.Append("updatePagination();");
            html.Append("}");

            html.Append("function searchItems() {");
            html.Append("const query = document.getElementById('itemSearch').value.toLowerCase();");
            html.Append("const quality = document.getElementById('qualityFilter').value;");
            html.Append("const category = document.getElementById('categoryFilter').value;");
            html.Append("const countEl = document.getElementById('itemsCount');");
            
            html.Append("if (allItems.length === 0) return;");

            html.Append("const filtered = allItems.filter(item => {");
            html.Append("  const matchesQuery = !query || ");
            html.Append("         item.id.toString().includes(query) || ");
            html.Append("         (item.name && item.name.toLowerCase().includes(query)) || ");
            html.Append("         (item.displayName && item.displayName.toLowerCase().includes(query));");
            html.Append("  const matchesQuality = !quality || item.quality == quality;");
            html.Append("  const matchesCategory = !category || item.category === category;");
            html.Append("  return matchesQuery && matchesQuality && matchesCategory;");
            html.Append("});");
            
            html.Append("countEl.textContent = '找到 ' + filtered.length + ' 个物品 (总数: ' + allItems.length + ')';");
            html.Append("renderItems(filtered, true);");
            html.Append("}");
            
            html.Append("function getTotalPages() { return Math.ceil(filteredItems.length / itemsPerPage); }");
            
            html.Append("function updatePagination() {");
            html.Append("var pagination = document.getElementById('itemsPagination');");
            html.Append("var pageInfo = document.getElementById('pageInfo');");
            html.Append("var totalPages = getTotalPages();");
            html.Append("if(totalPages <= 1) { pagination.style.display = 'none'; return; }");
            html.Append("pagination.style.display = 'flex';");
            html.Append("var startIdx = (currentPage - 1) * itemsPerPage + 1;");
            html.Append("var endIdx = Math.min(currentPage * itemsPerPage, filteredItems.length);");
            html.Append("pageInfo.textContent = '第 ' + currentPage + '/' + totalPages + ' 页 (' + startIdx + '-' + endIdx + ')';");
            html.Append("var btns = pagination.querySelectorAll('.page-btn');");
            html.Append("btns[0].disabled = currentPage === 1;");
            html.Append("btns[1].disabled = currentPage === 1;");
            html.Append("btns[2].disabled = currentPage === totalPages;");
            html.Append("btns[3].disabled = currentPage === totalPages;");
            html.Append("}");
            
            html.Append("function goToPage(page) {");
            html.Append("var totalPages = getTotalPages();");
            html.Append("if(page < 1) page = 1;");
            html.Append("if(page > totalPages) page = totalPages;");
            html.Append("if(page === currentPage) return;");
            html.Append("currentPage = page;");
            html.Append("renderItems(filteredItems, false);");
            html.Append("document.getElementById('itemsGrid').scrollIntoView({behavior: 'smooth', block: 'start'});");
            html.Append("}");
            
            html.Append("function changePageSize(size) {");
            html.Append("itemsPerPage = parseInt(size);");
            html.Append("localStorage.setItem('omni_itemsPerPage', itemsPerPage);");
            html.Append("currentPage = 1;");
            html.Append("renderItems(filteredItems, false);");
            html.Append("document.getElementById('itemsGrid').scrollIntoView({behavior: 'smooth', block: 'start'});");
            html.Append("}");
            
            html.Append("function debounce(func, wait) { let timeout; return function() { const context = this, args = arguments; clearTimeout(timeout); timeout = setTimeout(() => func.apply(context, args), wait); }; }");
            html.Append("const debouncedSearch = debounce(searchItems, 300);");
            
            html.Append("document.addEventListener('DOMContentLoaded', function() {");
            html.Append("  var pageSizeSelect = document.getElementById('pageSizeSelect');");
            html.Append("  if(pageSizeSelect) pageSizeSelect.value = itemsPerPage;");
            html.Append("});");
            
            html.Append("function openItemModal(id) {");
            html.Append("var item = allItems.find(function(i) { return i.id === id; });");
            html.Append("if(!item) { showToast('错误', '未找到物品信息', 'error'); return; }");
            html.Append("selectedItemId = id;");
            html.Append("document.getElementById('modalItemName').textContent = item.displayName || item.name;");
            html.Append("document.getElementById('modalItemCategory').textContent = translateCategory(item.category) + ' | ID: ' + item.id;");
            html.Append("var iconEl = document.getElementById('modalItemIcon');");
            html.Append("iconEl.innerHTML = item.icon ? '<img src=\"data:image/png;base64,' + item.icon + '\">' : '<i class=\"fa-solid fa-xmark\" style=\"font-size: 24px; color: var(--text-secondary);\"></i>';");
            html.Append("document.getElementById('itemQuantity').value = 1;");
            html.Append("document.getElementById('itemModal').classList.add('show');");
            html.Append("}");
            
            html.Append("function closeItemModal() {");
            html.Append("document.getElementById('itemModal').classList.remove('show');");
            html.Append("selectedItemId = null;");
            html.Append("}");
            
            html.Append("function adjustQuantity(delta) {");
            html.Append("var input = document.getElementById('itemQuantity');");
            html.Append("var val = parseInt(input.value) || 1;");
            html.Append("val = Math.max(1, Math.min(9999, val + delta));");
            html.Append("input.value = val;");
            html.Append("}");
            
            html.Append("var isAddingItem = false;");
            html.Append("function confirmAddItem() {");
            html.Append("if(isAddingItem || !selectedItemId) return;");
            html.Append("isAddingItem = true;");
            html.Append("var quantity = parseInt(document.getElementById('itemQuantity').value) || 1;");
            html.Append("quantity = Math.max(1, Math.min(9999, quantity));");
            html.Append("var item = allItems.find(function(i) { return i.id === selectedItemId; });");
            html.Append("var name = item ? (item.displayName || item.name) : 'ID:' + selectedItemId;");
            html.Append("fetch('/api/items/give', {");
            html.Append("  method: 'POST',");
            html.Append("  headers: {'Content-Type': 'application/x-www-form-urlencoded'},");
            html.Append("  body: 'id=' + selectedItemId + '&quantity=' + quantity");
            html.Append("}).then(function(r) { return r.json(); }).then(function(data) {");
            html.Append("  isAddingItem = false;");
            html.Append("  if(data.success) {");
            html.Append("    showToast('物品获取成功', '已添加 ' + quantity + ' 个 ' + name + ' 到背包', 'success');");
            html.Append("    closeItemModal();");
            html.Append("  } else {");
            html.Append("    showToast('获取失败', data.message || '无法添加物品', 'error');");
            html.Append("  }");
            html.Append("}).catch(function(err) {");
            html.Append("  isAddingItem = false;");
            html.Append("  console.error('[OmniGauge] Failed to give item:', err);");
            html.Append("  showToast('请求错误', err.message, 'error');");
            html.Append("});");
            html.Append("}");
        }
    }
}
